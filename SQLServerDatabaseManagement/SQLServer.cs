using System;
using System.Data;
using System.Data.SqlClient;

namespace SQLServerDatabaseManagement
{
    public  class SQLServer
    { 
        public  string ConnectionString { get; set; }

        public SqlConnection DbConnection { get; set; }

        public string GetConnectionString(string serverName, string database)
        {
            return string.Format(@"Data Source={0};Initial Catalog={1};Integrated Security=true", serverName, database);
        }

        public string GetConnectionString(string serverName, string username, string password, string database)
        {
            return string.Format(@"Persist Security Info=True;Data Source={0};Initial Catalog={1};User ID={2};Password={3}", serverName, database, username, password);
        }

        public SqlConnection OpenConnection()
        {
            DbConnection = new SqlConnection(ConnectionString);
            if (DbConnection.State == ConnectionState.Closed || DbConnection.State == ConnectionState.Broken)
            {
                DbConnection.Open();
            }
            return DbConnection;
        }

        public void CloseConnection()
        {
            if (DbConnection.State == ConnectionState.Open)
            {
                DbConnection.Close();
                DbConnection = null;
                ConnectionString = string.Empty;
            }
        }

        public DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var dtSet = new DataTable();
            if (DbConnection != null && !string.IsNullOrEmpty(DbConnection.ConnectionString) && DbConnection.State != ConnectionState.Closed)
            {
                using (SqlCommand command = DbConnection.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();
                    return dtSet;
                }
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandTimeout = 60000;
                    command.CommandText = query;
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtSet);
                    command.Parameters.Clear();
                }
            }
            return dtSet;
        }

        public bool CheckConnection(string connectionString)
        {
            try
            {
                var dbConnection = new SqlConnection(connectionString);
                if (dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken)
                {
                    dbConnection.Open();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public DataTable GetDatabase()
        {
            var query = @"select database_id, name, create_date, state_desc from sys.databases";
            return ExecuteDataTable(query);
        }

        public DataTable GetTables(string databaseName)
        {
            var query = string.Format(@"use {0}
                                        select a.TABLE_CATALOG, a.TABLE_SCHEMA, a.TABLE_NAME, b.type_desc as TABLE_TYPE, b.create_date 
                                        from INFORMATION_SCHEMA.TABLES a
                                        left join sys.tables b on a.TABLE_NAME = b.name
                                        where b.type = 'U'
                                        order by a.TABLE_NAME", databaseName);
            return ExecuteDataTable(query);
        }

        public DataTable GetTableColumns(string databaseName, string tableName)
        {
            var query = string.Format(@"use {0}
                                        select COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
                                        from INFORMATION_SCHEMA.COLUMNS 
                                        where TABLE_NAME = '{1}'", databaseName,tableName);
            return ExecuteDataTable(query);
        }

        public DataTable GetViews(string databaseName)
        {
            var query = string.Format(@"use {0}
                                        select a.TABLE_NAME, a.CHECK_OPTION, a.IS_UPDATABLE, a.TABLE_SCHEMA, a.TABLE_CATALOG, b.create_date 
                                        from INFORMATION_SCHEMA.VIEWS a
                                        inner join sys.views b on a.TABLE_NAME = b.name
                                        order by a.TABLE_NAME", databaseName);
            return ExecuteDataTable(query);
        }

        public DataTable GetFunctions(string databaseName)
        {
            var query = string.Format(@"use {0}
                                        select object_id as FUNC_ID, name as FUNC_NAME, create_date 
                                        from sys.objects 
                                        where type in ('FN', 'TF', 'IF')
                                        order by name", databaseName);
            return ExecuteDataTable(query);
        }

        public DataTable GetFunctionParams(string databaseName, int funcId)
        {
            var query = string.Format(@"use {0}
                                        select object_id as PARAM_ID, name as PARAM_NAME, TYPE_NAME(user_type_id) as PARAM_USER_TYPE, max_length as PARAM_MAX_LENGTH,
                                        case is_output when 0 then 'NO' else 'YES' end as PARAM_ISOUTPUT
                                        from sys.parameters
                                        where object_id = {1} and name <> ''", databaseName, funcId);
            return ExecuteDataTable(query);
        }

        public DataTable GetDataTypeReturnFunction(string databaseName, int funcId)
        {
            var query = string.Format(@"use {0}
                                        select object_id as PARAM_ID, name as PARAM_NAME, TYPE_NAME(user_type_id) as PARAM_USER_TYPE, max_length as PARAM_MAX_LENGTH
                                        from sys.parameters
                                        where object_id = {1} and name = ''", databaseName, funcId);
            return ExecuteDataTable(query);
        }

        public DataTable GetStoredProcedures(string databaseName)
        {
            var query = string.Format(@"use {0}
                                        Select id as STORE_ID, name as STORE_NAME, CONVERT(DATE, crdate) as CREATE_DATE 
                                        from sysobjects where type = 'P'
                                        order by name", databaseName);
            return ExecuteDataTable(query);
        }

        public DataTable GetStoredProcParams(string databaseName, int storeId)
        {
            var query = string.Format(@"use {0}
                                        select object_id as PARAM_ID, name as PARAM_NAME, TYPE_NAME(user_type_id) as PARAM_USER_TYPE, max_length as PARAM_MAX_LENGTH,
                                        case is_output when 0 then 'NO' else 'YES' end as PARAM_ISOUTPUT
                                        from sys.parameters
                                        where object_id = {1}", databaseName, storeId);
            return ExecuteDataTable(query);
        }

        public DataTable GetTriggers(string databaseName)
        {
            var query = string.Format(@"use {0}
                                        select object_id as id, name, SCHEMA_NAME(SCHEMA_ID) as schema_name, create_date 
                                        from sys.objects 
                                        where type = 'TR' order by name", databaseName);
            return ExecuteDataTable(query);
        }
    }
}
