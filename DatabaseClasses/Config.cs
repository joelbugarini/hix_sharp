using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace hix
{
    class Config
    {
        public string RDBMS { get; set; }
        public string Database { get; set; }
        public string Server { get; set; }
        public string Project { get; set; }
        public bool WinAuth { get; set; }
        public string User { get; set; }
        public string Password { get; set; }        
        public string DatabasePath { get; set; }

        public string GetSqlCon()
        {
            if (this.WinAuth)
            {
                if (string.IsNullOrEmpty(this.DatabasePath))
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
                return @"Server=" + this.Server + ";Database=" + this.Database + ";User Id=" + this.User + ";Password=" + this.Password + ";";
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
            Config conf = new Config();
            string text = System.IO.File.ReadAllText("config.json");
            string[] t = text
                .Replace("{", "")
                .Replace("}", "")
                .Replace("\"", "")
                .Replace("\n", "")
                .Replace("\r", "")                
                .Replace(" ", "")
                .Split(',');
            foreach (string prop in t)
            {
                string val = "";
                if (prop.Split(':').Length > 2)
                {
                    string[] tmp = Util.Tail(prop.Split(':'));
                    val = string.Join(":", tmp);
                }
                else {
                    val = prop.Split(':').Length == 2 ? prop.Split(':')[1] : "";
                }
                
                switch (prop.Split(':')[0])
                {
                    case "RDBMS":
                        conf.RDBMS = val;
                        break;
                    case "Database":
                        conf.Database = val;
                        break;
                    case "Server":
                        conf.Server = val;
                        break;
                    case "Project":
                        conf.Project = val;
                        break;
                    case "WinAuth":
                        conf.WinAuth = Convert.ToBoolean(val);
                        break;
                    case "User":
                        conf.User = val;
                        break;
                    case "Password":
                        conf.Password = val;
                        break;
                    case "DatabasePath":
                        conf.DatabasePath = val;
                        break;

                }
               
            }
            return conf;
        }

    }
}
