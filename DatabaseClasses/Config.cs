using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Json;
using Newtonsoft.Json;

namespace hix
{
    class Config
    {
        public string Project { get; set; }
        public string Models { get; set; }

        //Conect to Database
        public Schema GenerateDb(Config config)
        {
            Schema db = new Schema();
            db.Name = config.Project + "_Schema";
            db.Tables = GetTables(config);
            foreach (Table table in db.Tables)
            {
                table.Columns = GetColumns(config, table.Name.Replace(".json", ""));
            }

            return db;
        }

        //Fill Table
        public List<Table> GetTables(Config config)
        {
            List<Table> Tables = new List<Table>();

            DirectoryInfo d = new DirectoryInfo(config.Models);
            FileInfo[] Files = d.GetFiles("*.json");

            foreach (FileInfo file in Files)
            {
                Table table = new Table();
                table.Name = file.Name.Replace(".json","");
                Tables.Add(table);
            }

            return Tables;
        }

        //Fill Columns
        public List<Column> GetColumns(Config config, string TableName)
        {
           var fileName = TableName + ".json";
           List<Column> Columns = new List<Column>();
           string allText = File.ReadAllText(Path.Combine(config.Models, fileName));
           Columns = JsonConvert.DeserializeObject< List<Column>>(allText);

           return Columns;
        }


        public void GetTablesConsole(Config config)
        {

            List<Table> Tables = new List<Table>();

            DirectoryInfo d = new DirectoryInfo(config.Models);
            FileInfo[] Files = d.GetFiles("*.json");

            foreach (FileInfo file in Files)
            {
                Console.WriteLine(file.Name);
            }
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
                else
                {
                    val = prop.Split(':').Length == 2 ? prop.Split(':')[1] : "";
                }

                switch (prop.Split(':')[0])
                {
                    case "Project":
                        conf.Project = val;
                        break;
                    case "Models":
                        conf.Models = val;
                        break;

                }

            }
            return conf;
        }
    }
}
