using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aven
{
    class Config
    {
        private string database;

        public string Database
        {
            get { return database; }
            set { database = value; }
        }
        private string server;

        public string Server
        {
            get { return server; }
            set { server = value; }
        }
        private string project;

        public string Project
        {
            get { return project; }
            set { project = value; }
        }
        private bool winAuth;

        public bool WinAuth
        {
            get { return winAuth; }
            set { winAuth = value; }
        }
        private string user;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string databasePath;

        public string DatabasePath
        {
            get { return databasePath; }
            set { databasePath = value; }
        }

        public string GetSqlCon()
        {
            if (this.WinAuth)
            {
                if(string.IsNullOrWhiteSpace(this.DatabasePath))
                {
                    return @"Data Source=" + this.Server + ";Initial Catalog=" + this.Database + ";persist security info=True;Integrated Security=SSPI;";
                }
                else
                {
                    return @"Server=" + this.Server + ";Integrated Security=true;AttachDbFileName=" + this.DatabasePath + ";";
                }
                

            }
            else 
            {
                return @"Server="+this.Server+";Database="+this.Database+";User Id="+this.User+";Password="+this.Password+";";
            }
        }
        public string GetAppName()
        {
            return "";
        }


        //Conect to Database
        public Database GenerateDb(Config config)
        {
            Database db = new Database();
            db.Name = config.Database;
            db.Tables = GetTables(config);
            foreach (Table table in db.Tables)
            {
                table.Columns = GetColumns(config, table.Name);
            }

            return db;
        }

        //Fill Table
        public List<Table> GetTables(Config config)
        {
            List<Table> Tables = new List<Table>();
            string queryString = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
            using (SqlConnection connection = new SqlConnection(config.GetSqlCon()))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open(); SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Table table = new Table();
                        table.Name = reader["TABLE_NAME"].ToString();
                        Tables.Add(table);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            return Tables;
        }

        //Fill Columns
        public List<Column> GetColumns(Config config, string ColumnName)
        {

            List<Column> Columns = new List<Column>();
            string queryString = "SELECT * FROM " + config.Database + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + ColumnName + "'";
            using (SqlConnection connection = new SqlConnection(config.GetSqlCon()))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open(); SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Column col = new Column();
                        col.Name = reader["COLUMN_NAME"].ToString();
                        col.Type = reader["DATA_TYPE"].ToString();
                        Columns.Add(col);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            return Columns;
        }

        //Read the config file
        public Config ReadConfig()
        {
            string text = System.IO.File.ReadAllText("config.json");
            Config readedConfig = JsonConvert.DeserializeObject<Config>(text);
            return readedConfig;
        }
    }
}
