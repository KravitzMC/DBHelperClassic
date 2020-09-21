using System;
using System.Collections;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SQLServerHelper.AspNetCore
{
    public partial class sqlHelper_static
    {
        public static string SourceConnection;
        public static string DestinationConnection;

        public static bool TestConnectDB(string str)
        {
            using (SqlConnection conn = new SqlConnection(str))
            {
                try
                {
                    conn.Open();
                    conn.Close();
                    return true;
                }
                catch { return false; }
            }
        }
        public static DataRow GetRow(DataTable table, string fieldid, string fieldName)
        {
            DataRow row = table.NewRow();
            row[fieldid] = "";
            row[fieldName] = "";
            return row;
        }


        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(SourceConnection, cmdType, cmdText, commandParameters);
        }

        public static int ExecuteNonQuery(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(SourceConnection, CommandType.Text, cmdText, commandParameters);
        }

        public static int ExecuteNonQueryProc(string StoredProcedureName, params SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(SourceConnection, CommandType.StoredProcedure, StoredProcedureName, commandParameters);
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
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

        public static SqlDataReader ExecuteReader(SqlConnection conn, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(SourceConnection, CommandType.Text, cmdText, commandParameters);
        }

        public static SqlDataReader ExecuteReader(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(SourceConnection, CommandType.Text, cmdText, commandParameters);
        }

        public static SqlDataReader ExecuteReaderProc(string StoredProcedureName, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(SourceConnection, CommandType.StoredProcedure, StoredProcedureName, commandParameters);
        }

        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(SourceConnection, cmdType, cmdText, commandParameters);
        }

        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(SourceConnection, CommandType.Text, cmdText, commandParameters);
        }

        public static object ExecuteScalarProc(string StoredProcedureName, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(SourceConnection, CommandType.StoredProcedure, StoredProcedureName, commandParameters);
        }

        public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(SourceConnection, cmdType, cmdText, commandParameters);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
            if (cachedParms == null)
                return null;
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
            return clonedParms;
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
                    cmd.Parameters.Add(parm);
            }
        }

        public static DataTable ReadTable(SqlTransaction transaction, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, cmdType, cmdText, commandParameters);
            DataTable dt = HelperBase.ReadTable(cmd);
            cmd.Parameters.Clear();
            return dt;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(SourceConnection);
        }

        public static DataTable ReadTable(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ReadTable(connection, cmdType, cmdText, commandParameters);
            }
        }
        public static DataTable ReadTable(string cmdText, params SqlParameter[] commandParameters)
        {
            return ReadTable(CommandType.Text, cmdText, commandParameters);
        }
        public static DataTable ReadTable(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return ReadTable(SourceConnection, cmdType, cmdText, commandParameters);
        }

        public static DataTable ReadTable(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            DataTable dt = HelperBase.ReadTable(cmd);
            cmd.Parameters.Clear();
            return dt;
        }

        public static SqlParameter CreateInputParameter(string paramName, SqlDbType dbtype, object value)
        {
            return CreateParameter(ParameterDirection.Input, paramName, dbtype, 0, value);
        }
        public static SqlParameter CreateInputParameter(string paramName, SqlDbType dbtype, int size, object value)
        {
            return CreateParameter(ParameterDirection.Input, paramName, dbtype, size, value);
        }

        public static SqlParameter CreateOutputParameter(string paramName, SqlDbType dbtype)
        {
            return CreateParameter(ParameterDirection.Output, paramName, dbtype, 0, DBNull.Value);
        }

        public static SqlParameter CreateOutputParameter(string paramName, SqlDbType dbtype, int size)
        {
            return CreateParameter(ParameterDirection.Output, paramName, dbtype, size, DBNull.Value);
        }

        public static SqlParameter CreateParameter(ParameterDirection direction, string paramName, SqlDbType dbtype, int size, object value)
        {
            SqlParameter param = new SqlParameter(paramName, dbtype, size);
            param.Value = value;
            param.Direction = direction;
            return param;
        }
    }
}
