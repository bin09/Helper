using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Helper.DBUtility
{
    public class SQLHelper2
    {
        /// <summary>
        /// 执行ExecuteScalar
        /// </summary>
        /// <param name="connString">数据库连接串</param>
        /// <param name="cmdType">Sql语句类型</param>
        /// <param name="cmdText">Sql语句</param>
        /// <param name="cmdParms">Parm数组</param>
        /// <returns>返回第一行第一列记录值</returns>
        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                if (cmdParms != null)
                {
                    foreach (SqlParameter parm in cmdParms)
                    {
                        if (parm.Value == null)
                        {
                            parm.Value = DBNull.Value;
                        }
                        cmd.Parameters.Add(parm);
                    }
                }
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                conn.Close();
                return val;
            }
        }

        public static SqlDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }


        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
                return val;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if (parm.Value == null)
                    {
                        parm.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parm);
                }
            }
        }

        public static DataSet GetDateSet(string connString, string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                System.Data.SqlClient.SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand = new SqlCommand(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetDateTable(string connString, string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                System.Data.SqlClient.SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand = new SqlCommand(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public static int ExecuteNonQuery(string connString, string sql)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                int val = cmd.ExecuteNonQuery();
                conn.Close();
                return val;
            }
        }

        public static int ExecuteScalar(string connString, string sql)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                object obj = cmd.ExecuteScalar();
                int val = 0;
                if (obj != DBNull.Value && obj != null)
                {
                    val = Int32.Parse(cmd.ExecuteScalar().ToString());
                }
                conn.Close();
                return val;
            }
        }
    }
}
