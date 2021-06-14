using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Jsonite;

namespace hix
{
    class Config
    {
        public string Project { get; set; }
        public string Models { get; set; }

        //Conect to Database
        public Schema GenerateSchema(Config config)
        {
            Schema db = new Schema();
            db.Name = config.Project + "_Schema";
            db.Models = GetModels(config);
            foreach (Model table in db.Models)
            {
                table.Props = GetProperties(config, table.Name.Replace(".json", ""));
            }

            return db;
        }

        //Fill Table
        public List<Model> GetModels(Config config)
        {
            List<Model> Tables = new List<Model>();

            DirectoryInfo d = new DirectoryInfo(config.Models);
            FileInfo[] Files = d.GetFiles("*.json");

            foreach (FileInfo file in Files)
            {
                Model table = new Model();
                table.Name = file.Name.Replace(".json","");
                Tables.Add(table);
            }

            return Tables;
        }

        //Fill Columns
        public List<Property> GetProperties(Config config, string TableName)
        {
           var fileName = TableName + ".json";
           List<Property> Columns = new List<Property>();
           string allText = File.ReadAllText(Path.Combine(config.Models, fileName));

            var res = Json.Deserialize(allText);

            foreach (JsonObject item in res as JsonArray) 
            {
                object n = null;
                if (!item.TryGetValue("Name", out n))
                    Console.WriteLine("Please, provide 'Name' for each property");

                object t = null;
                if (!item.TryGetValue("Type", out t))
                    Console.WriteLine("Please, provide 'Type' for each property");

                if(n == null || t == null)
                    Console.WriteLine("Error found in: " + item.ToString());

                Columns.Add(new Property() { Name = n as string, Type = t as string });
            }


           return Columns;
        }


        public void GetModelsConsole(Config config)
        {

            List<Model> Tables = new List<Model>();

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
            string text = System.IO.File.ReadAllText("config.json");

            JsonObject item = Json.Deserialize(text) as JsonObject;

            if (!item.TryGetValue("Project", out object p))
                Console.WriteLine("Please, provide 'Project' in the config file");

            if (!item.TryGetValue("Models", out object m))
                Console.WriteLine("Please, provide 'Models' in the config file");

            Project = p as string;
            Models = m as string;
           
            return this;
        }
    }
}
