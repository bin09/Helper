using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Helper.DBUtility
{
    public class BaseSQL2
    {
        private readonly string connectionString = null;
        /// <summary>
        /// 取得Dataset数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecuteDateSet(string sql)
        {
            return SQLHelper2.GetDateSet(connectionString, sql);
        }
        /// <summary>
        /// 取得DataTable数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return SQLHelper2.GetDateTable(connectionString, sql);
        }
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            return SQLHelper2.ExecuteNonQuery(connectionString, sql);
        }
        public int ExecuteNonQuery(CommandType commandType, string sql, SqlParameter[] parameters)
        {
            return SQLHelper2.ExecuteNonQuery(connectionString, commandType, sql, parameters);
        }
        /// <summary>
        /// 返回第一行第一列的值(返回DBNull.Value不为空的值，否则返回0)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteScalar(string sql)
        {
            return SQLHelper2.ExecuteScalar(connectionString, sql);
        }
        public object ExecuteScalar(CommandType commandType, string sql, SqlParameter[] parameters)
        {
            return SQLHelper2.ExecuteScalar(connectionString, commandType, sql, parameters);
        }
        /// <summary>
        /// DataReader,读取数据
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(CommandType commandType, string sql, SqlParameter[] parameters)
        {
            return SQLHelper2.ExecuteReader(connectionString, commandType, sql, parameters);
        }
    }
}
