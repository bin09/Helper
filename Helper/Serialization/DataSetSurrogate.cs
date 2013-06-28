using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Globalization;

namespace Helper.Serialization
{
    [Serializable]
    public class DataSetSurrogate
    {
        //DataSet properties
        private string _datasetName;
        private string _namespace;
        private string _prefix;
        private bool _caseSensitive;
        private CultureInfo _locale;
        private bool _enforceConstraints;

        //ForeignKeyConstraints
        private ArrayList _fkConstraints;//An ArrayList of foreign key constraints :  [constraintName]->[parentTableIndex, parentcolumnIndexes]->[childTableIndex, childColumnIndexes]->[AcceptRejectRule, UpdateRule, Delete]->[extendedProperties]

        //Relations
        private ArrayList _relations;//An ArrayList of foreign key constraints : [relationName]->[parentTableIndex, parentcolumnIndexes]->[childTableIndex, childColumnIndexes]->[Nested]->[extendedProperties]

        //ExtendedProperties
        private Hashtable _extendedProperties;

        //Columns and Rows
        private DataTableSurrogate[] _dataTableSurrogates;

        /*
            Constructs a DataSetSurrogate object from a DataSet.
        */
        public DataSetSurrogate(DataSet ds)
        {
            if (ds == null)
            {
                throw new ArgumentNullException("The parameter dataset is null");
            }

            //DataSet properties
            _datasetName = ds.DataSetName;
            _namespace = ds.Namespace;
            _prefix = ds.Prefix;
            _caseSensitive = ds.CaseSensitive;
            _locale = ds.Locale;
            _enforceConstraints = ds.EnforceConstraints;

            //Tables, Columns, Rows
            _dataTableSurrogates = new DataTableSurrogate[ds.Tables.Count];
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                _dataTableSurrogates[i] = new DataTableSurrogate(ds.Tables[i]);
            }

            //ForeignKeyConstraints
            _fkConstraints = GetForeignKeyConstraints(ds);

            //Relations
            _relations = GetRelations(ds);

            //ExtendedProperties
            _extendedProperties = new Hashtable();
            if (ds.ExtendedProperties.Keys.Count > 0)
            {
                foreach (object propertyKey in ds.ExtendedProperties.Keys)
                {
                    _extendedProperties.Add(propertyKey, ds.ExtendedProperties[propertyKey]);
                }
            }
        }

        /*
            Constructs a DataSet from the DataSetSurrogate object. This can be used after the user recieves a Surrogate object over the wire and wished to construct a DataSet from it.
        */
        public DataSet ConvertToDataSet()
        {
            DataSet ds = new DataSet();
            ReadSchemaIntoDataSet(ds);
            ReadDataIntoDataSet(ds);
            return ds;
        }

        /*
            Reads the schema into the dataset from the DataSetSurrogate object.
        */
        public void ReadSchemaIntoDataSet(DataSet ds)
        {
            if (ds == null)
            {
                throw new ArgumentNullException("The dataset parameter cannot be null");
            }

            //DataSet properties
            ds.DataSetName = _datasetName;
            ds.Namespace = _namespace;
            ds.Prefix = _prefix;
            ds.CaseSensitive = _caseSensitive;
            ds.Locale = _locale;
            ds.EnforceConstraints = _enforceConstraints;

            //Tables, Columns
            Debug.Assert(_dataTableSurrogates != null);
            foreach (DataTableSurrogate dataTableSurrogate in _dataTableSurrogates)
            {
                DataTable dt = new DataTable();
                dataTableSurrogate.ReadSchemaIntoDataTable(dt);
                ds.Tables.Add(dt);
            }

            //ForeignKeyConstraints
            SetForeignKeyConstraints(ds, _fkConstraints);

            //Relations
            SetRelations(ds, _relations);

            //Set ExpressionColumns        
            Debug.Assert(_dataTableSurrogates != null);
            Debug.Assert(ds.Tables.Count == _dataTableSurrogates.Length);
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                DataTableSurrogate dataTableSurrogate = _dataTableSurrogates[i];
                dataTableSurrogate.SetColumnExpressions(dt);
            }

            //ExtendedProperties
            Debug.Assert(_extendedProperties != null);
            if (_extendedProperties.Keys.Count > 0)
            {
                foreach (object propertyKey in _extendedProperties.Keys)
                {
                    ds.ExtendedProperties.Add(propertyKey, _extendedProperties[propertyKey]);
                }
            }
        }

        /*
            Reads the data into the dataset from the DataSetSurrogate object.
        */
        public void ReadDataIntoDataSet(DataSet ds)
        {
            if (ds == null)
            {
                throw new ArgumentNullException("The dataset parameter cannot be null");
            }

            //Suppress  read-only columns and constraint rules when loading the data
            ArrayList readOnlyList = SuppressReadOnly(ds);
            ArrayList constraintRulesList = SuppressConstraintRules(ds);

            //Rows
            Debug.Assert(IsSchemaIdentical(ds));
            Debug.Assert(_dataTableSurrogates != null);
            Debug.Assert(ds.Tables.Count == _dataTableSurrogates.Length);
            bool enforceConstraints = ds.EnforceConstraints;
            ds.EnforceConstraints = false;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                DataTableSurrogate dataTableSurrogate = _dataTableSurrogates[i];
                dataTableSurrogate.ReadDataIntoDataTable(ds.Tables[i], false);
            }
            ds.EnforceConstraints = enforceConstraints;

            //Reset read-only columns and constraint rules back after loading the data
            ResetReadOnly(ds, readOnlyList);
            ResetConstraintRules(ds, constraintRulesList);
        }

        /*
            Gets foreignkey constraints availabe on the tables in the dataset.
            ***Serialized foreign key constraints format : [constraintName]->[parentTableIndex, parentcolumnIndexes]->[childTableIndex, childColumnIndexes]->[AcceptRejectRule, UpdateRule, Delete]->[extendedProperties]***
        */
        private ArrayList GetForeignKeyConstraints(DataSet ds)
        {
            Debug.Assert(ds != null);

            ArrayList constraintList = new ArrayList();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                for (int j = 0; j < dt.Constraints.Count; j++)
                {
                    Constraint c = dt.Constraints[j];
                    ForeignKeyConstraint fk = c as ForeignKeyConstraint;
                    if (fk != null)
                    {
                        string constraintName = c.ConstraintName;
                        int[] parentInfo = new int[fk.RelatedColumns.Length + 1];
                        parentInfo[0] = ds.Tables.IndexOf(fk.RelatedTable);
                        for (int k = 1; k < parentInfo.Length; k++)
                        {
                            parentInfo[k] = fk.RelatedColumns[k - 1].Ordinal;
                        }

                        int[] childInfo = new int[fk.Columns.Length + 1];
                        childInfo[0] = i;//Since the constraint is on the current table, this is the child table.
                        for (int k = 1; k < childInfo.Length; k++)
                        {
                            childInfo[k] = fk.Columns[k - 1].Ordinal;
                        }

                        ArrayList list = new ArrayList();
                        list.Add(constraintName);
                        list.Add(parentInfo);
                        list.Add(childInfo);
                        list.Add(new int[] { (int)fk.AcceptRejectRule, (int)fk.UpdateRule, (int)fk.DeleteRule });
                        Hashtable extendedProperties = new Hashtable();
                        if (fk.ExtendedProperties.Keys.Count > 0)
                        {
                            foreach (object propertyKey in fk.ExtendedProperties.Keys)
                            {
                                extendedProperties.Add(propertyKey, fk.ExtendedProperties[propertyKey]);
                            }
                        }
                        list.Add(extendedProperties);

                        constraintList.Add(list);
                    }
                }
            }
            return constraintList;
        }

        /*
            Adds foreignkey constraints to the tables in the dataset. The arraylist contains the serialized format of the foreignkey constraints.
            ***Deserialize the foreign key constraints format : [constraintName]->[parentTableIndex, parentcolumnIndexes]->[childTableIndex, childColumnIndexes]->[AcceptRejectRule, UpdateRule, Delete]->[extendedProperties]***
        */
        private void SetForeignKeyConstraints(DataSet ds, ArrayList constraintList)
        {
            Debug.Assert(ds != null);
            Debug.Assert(constraintList != null);

            foreach (ArrayList list in constraintList)
            {
                Debug.Assert(list.Count == 5);
                string constraintName = (string)list[0];
                int[] parentInfo = (int[])list[1];
                int[] childInfo = (int[])list[2];
                int[] rules = (int[])list[3];
                Hashtable extendedProperties = (Hashtable)list[4];

                //ParentKey Columns.
                Debug.Assert(parentInfo.Length >= 1);
                DataColumn[] parentkeyColumns = new DataColumn[parentInfo.Length - 1];
                for (int i = 0; i < parentkeyColumns.Length; i++)
                {
                    Debug.Assert(ds.Tables.Count > parentInfo[0]);
                    Debug.Assert(ds.Tables[parentInfo[0]].Columns.Count > parentInfo[i + 1]);
                    parentkeyColumns[i] = ds.Tables[parentInfo[0]].Columns[parentInfo[i + 1]];
                }

                //ChildKey Columns.
                Debug.Assert(childInfo.Length >= 1);
                DataColumn[] childkeyColumns = new DataColumn[childInfo.Length - 1];
                for (int i = 0; i < childkeyColumns.Length; i++)
                {
                    Debug.Assert(ds.Tables.Count > childInfo[0]);
                    Debug.Assert(ds.Tables[childInfo[0]].Columns.Count > childInfo[i + 1]);
                    childkeyColumns[i] = ds.Tables[childInfo[0]].Columns[childInfo[i + 1]];
                }

                //Create the Constraint.
                ForeignKeyConstraint fk = new ForeignKeyConstraint(constraintName, parentkeyColumns, childkeyColumns);
                Debug.Assert(rules.Length == 3);
                fk.AcceptRejectRule = (AcceptRejectRule)rules[0];
                fk.UpdateRule = (Rule)rules[1];
                fk.DeleteRule = (Rule)rules[2];

                //Extended Properties.
                Debug.Assert(extendedProperties != null);
                if (extendedProperties.Keys.Count > 0)
                {
                    foreach (object propertyKey in extendedProperties.Keys)
                    {
                        fk.ExtendedProperties.Add(propertyKey, extendedProperties[propertyKey]);
                    }
                }

                //Add the constraint to the child datatable.
                Debug.Assert(ds.Tables.Count > childInfo[0]);
                ds.Tables[childInfo[0]].Constraints.Add(fk);
            }
        }

        /*
            Gets relations from the dataset.
            ***Serialized relations format : [relationName]->[parentTableIndex, parentcolumnIndexes]->[childTableIndex, childColumnIndexes]->[Nested]->[extendedProperties]***
        */
        private ArrayList GetRelations(DataSet ds)
        {
            Debug.Assert(ds != null);

            ArrayList relationList = new ArrayList();
            foreach (DataRelation rel in ds.Relations)
            {
                string relationName = rel.RelationName;
                int[] parentInfo = new int[rel.ParentColumns.Length + 1];
                parentInfo[0] = ds.Tables.IndexOf(rel.ParentTable);
                for (int j = 1; j < parentInfo.Length; j++)
                {
                    parentInfo[j] = rel.ParentColumns[j - 1].Ordinal;
                }

                int[] childInfo = new int[rel.ChildColumns.Length + 1];
                childInfo[0] = ds.Tables.IndexOf(rel.ChildTable);
                for (int j = 1; j < childInfo.Length; j++)
                {
                    childInfo[j] = rel.ChildColumns[j - 1].Ordinal;
                }

                ArrayList list = new ArrayList();
                list.Add(relationName);
                list.Add(parentInfo);
                list.Add(childInfo);
                list.Add(rel.Nested);
                Hashtable extendedProperties = new Hashtable();
                if (rel.ExtendedProperties.Keys.Count > 0)
                {
                    foreach (object propertyKey in rel.ExtendedProperties.Keys)
                    {
                        extendedProperties.Add(propertyKey, rel.ExtendedProperties[propertyKey]);
                    }
                }
                list.Add(extendedProperties);

                relationList.Add(list);
            }
            return relationList;
        }

        /*
            Adds relations to the dataset. The arraylist contains the serialized format of the relations.
            ***Deserialize the relations format : [relationName]->[parentTableIndex, parentcolumnIndexes]->[childTableIndex, childColumnIndexes]->[Nested]->[extendedProperties]***
        */
        private void SetRelations(DataSet ds, ArrayList relationList)
        {
            Debug.Assert(ds != null);
            Debug.Assert(relationList != null);

            foreach (ArrayList list in relationList)
            {
                Debug.Assert(list.Count == 5);
                string relationName = (string)list[0];
                int[] parentInfo = (int[])list[1];
                int[] childInfo = (int[])list[2];
                bool isNested = (bool)list[3];
                Hashtable extendedProperties = (Hashtable)list[4];

                //ParentKey Columns.
                Debug.Assert(parentInfo.Length >= 1);
                DataColumn[] parentkeyColumns = new DataColumn[parentInfo.Length - 1];
                for (int i = 0; i < parentkeyColumns.Length; i++)
                {
                    Debug.Assert(ds.Tables.Count > parentInfo[0]);
                    Debug.Assert(ds.Tables[parentInfo[0]].Columns.Count > parentInfo[i + 1]);
                    parentkeyColumns[i] = ds.Tables[parentInfo[0]].Columns[parentInfo[i + 1]];
                }

                //ChildKey Columns.
                Debug.Assert(childInfo.Length >= 1);
                DataColumn[] childkeyColumns = new DataColumn[childInfo.Length - 1];
                for (int i = 0; i < childkeyColumns.Length; i++)
                {
                    Debug.Assert(ds.Tables.Count > childInfo[0]);
                    Debug.Assert(ds.Tables[childInfo[0]].Columns.Count > childInfo[i + 1]);
                    childkeyColumns[i] = ds.Tables[childInfo[0]].Columns[childInfo[i + 1]];
                }

                //Create the Relation, without any constraints[Assumption: The constraints are added earlier than the relations]
                DataRelation rel = new DataRelation(relationName, parentkeyColumns, childkeyColumns, false);
                rel.Nested = isNested;

                //Extended Properties.
                Debug.Assert(extendedProperties != null);
                if (extendedProperties.Keys.Count > 0)
                {
                    foreach (object propertyKey in extendedProperties.Keys)
                    {
                        rel.ExtendedProperties.Add(propertyKey, extendedProperties[propertyKey]);
                    }
                }

                //Add the relations to the dataset.
                ds.Relations.Add(rel);
            }
        }

        /*
            Suppress the read-only property and returns an arraylist of read-only columns.
        */
        private ArrayList SuppressReadOnly(DataSet ds)
        {
            Debug.Assert(ds != null);

            ArrayList readOnlyList = new ArrayList();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].Expression == String.Empty && dt.Columns[j].ReadOnly == true)
                    {
                        dt.Columns[j].ReadOnly = false;
                        readOnlyList.Add(new int[] { i, j });
                    }
                }
            }
            return readOnlyList;
        }

        /*
            Suppress the foreign key constraint rules and returns an arraylist of the existing foreignkey constraint rules.
        */
        private ArrayList SuppressConstraintRules(DataSet ds)
        {
            Debug.Assert(ds != null);

            ArrayList constraintRulesList = new ArrayList();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dtChild = ds.Tables[i];
                for (int j = 0; j < dtChild.Constraints.Count; j++)
                {
                    Constraint c = dtChild.Constraints[j];
                    if (c is ForeignKeyConstraint)
                    {
                        ForeignKeyConstraint fk = (ForeignKeyConstraint)c;
                        ArrayList list = new ArrayList();
                        list.Add(new int[] { i, j });
                        list.Add(new int[] { (int)fk.AcceptRejectRule, (int)fk.UpdateRule, (int)fk.DeleteRule });
                        constraintRulesList.Add(list);

                        fk.AcceptRejectRule = AcceptRejectRule.None;
                        fk.UpdateRule = Rule.None;
                        fk.DeleteRule = Rule.None;
                    }
                }
            }
            return constraintRulesList;
        }

        /*
            Resets the read-only columns on the datatable based on the input readOnly list.
        */
        private void ResetReadOnly(DataSet ds, ArrayList readOnlyList)
        {
            Debug.Assert(ds != null);
            Debug.Assert(readOnlyList != null);

            foreach (object o in readOnlyList)
            {
                int[] indicesArr = (int[])o;

                Debug.Assert(indicesArr.Length == 2);
                int tableIndex = indicesArr[0];
                int columnIndex = indicesArr[1];

                Debug.Assert(ds.Tables.Count > tableIndex);
                Debug.Assert(ds.Tables[tableIndex].Columns.Count > columnIndex);

                DataColumn dc = ds.Tables[tableIndex].Columns[columnIndex];
                Debug.Assert(dc != null);

                dc.ReadOnly = true;
            }
        }

        /*
            Resets the foreignkey constraint rules on the dataset based on the input constraint rules list.
        */
        private void ResetConstraintRules(DataSet ds, ArrayList constraintRulesList)
        {
            Debug.Assert(ds != null);
            Debug.Assert(constraintRulesList != null);

            foreach (ArrayList list in constraintRulesList)
            {
                Debug.Assert(list.Count == 2);
                int[] indicesArr = (int[])list[0];
                int[] rules = (int[])list[1];

                Debug.Assert(indicesArr.Length == 2);
                int tableIndex = indicesArr[0];
                int constraintIndex = indicesArr[1];

                Debug.Assert(ds.Tables.Count > tableIndex);
                DataTable dtChild = ds.Tables[tableIndex];

                Debug.Assert(dtChild.Constraints.Count > constraintIndex);
                ForeignKeyConstraint fk = (ForeignKeyConstraint)dtChild.Constraints[constraintIndex];

                Debug.Assert(rules.Length == 3);
                fk.AcceptRejectRule = (AcceptRejectRule)rules[0];
                fk.UpdateRule = (Rule)rules[1];
                fk.DeleteRule = (Rule)rules[2];
            }
        }

        /*
            Checks whether the dataset name and namespaces are as expected and the tables count is right.
        */
        private bool IsSchemaIdentical(DataSet ds)
        {
            Debug.Assert(ds != null);
            if (ds.DataSetName != _datasetName || ds.Namespace != _namespace)
            {
                return false;
            }
            Debug.Assert(_dataTableSurrogates != null);
            if (ds.Tables.Count != _dataTableSurrogates.Length)
            {
                return false;
            }
            return true;
        }
    } 
}
