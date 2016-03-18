using System;
using System.Collections.Generic;

using System.Text;

using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace hix
{
    class MainConsole
    {
        static void Main(string[] args)
        {            
            //Validate args
            if (IsGenerateOnly(args))
            {
                //Read Config File
                Config config = new Config().ReadConfig();

                //Get the DB schema
                Console.WriteLine("hix Class Generator is connecting to Database " + config.Server + "\\" + config.Database);

                //try
                //{
                    Database db = config.GenerateDb(config);

                    //Read File
                    
                    Console.WriteLine("Reading the " + args[1] + " File");
                    RootNode rootNode = new RootNode();
                    rootNode.Create(args[1], db);

                //Map File to Generate Code

                foreach (TableNode tableNode in rootNode.TableNodes)
                {
                    foreach (Table table in db.Tables)
                    {

                        table.Content = tableNode.Scheme;
                        foreach (ColumnNode columnNode in tableNode.ColumnNodes)
                        {
                            string dataColumnNode = "";
                            if (columnNode.Type == "notype")
                            {
                                foreach (Column column in table.Columns)
                                {

                                    dataColumnNode += columnNode.Scheme.Replace(
                                            "[[column.name]]", column.Name).Replace(
                                            "[[column.type]]", column.Type).Replace(
                                            "[[project.name]]", config.Project).Replace(
                                            "[[database.name]]", config.Database);
                                }
                            }
                            else
                            {
                                foreach (Column column in table.Columns)
                                {
                                    if (columnNode.Type == column.Type)
                                    {
                                        dataColumnNode += columnNode.Scheme.Replace(
                                                "[[column.name]]", column.Name).Replace(
                                                "[[column.type]]", column.Type).Replace(
                                                "[[project.name]]", config.Project).Replace(
                                                "[[database.name]]", config.Database);
                                    }
                                }
                            }

                            
                            table.Content = ReplaceFirst(table.Content, "[[columns]]", dataColumnNode).Replace(
                                    "[[table.name]]", table.Name).Replace(
                                    "[[project.name]]", config.Project).Replace(
                                    "[[database.name]]", config.Database);

                        }
                    }

                }
                    
                    if (IsTableNextToRoot(rootNode.Original))
                    {
                        foreach (Table table in db.Tables)
                        {
                            string file = Path.GetFileNameWithoutExtension(args[1]);
                            string extension = Path.GetExtension(args[1]);
                            (new FileInfo("output/"+file + "/")).Directory.Create();
                            CreateFile("output/" + file + "/" + file + table.Name + extension, table.Content);
                            Console.WriteLine("output/" + file + "/" + file + table.Name + extension + " Created");
                        }
                    }
                    else
                    {
                        string document = "";
                        foreach (Table table in db.Tables)
                        {
                            document += table.Content;
                        }
                        document = rootNode.Scheme.Replace("[[table.container]]", document)
                                                  .Replace("[[project.name]]", config.Project)
                                                  .Replace("[[database.name]]", config.Database);

                        string file = Path.GetFileNameWithoutExtension(args[1]);
                        string extension = Path.GetExtension(args[1]);
                        (new FileInfo("output/" + file + "/")).Directory.Create();
                        CreateFile("output/" + file + "/" + file + extension, document);
                        Console.WriteLine("output/" + file + "/" + file + extension + " Created");

                    }

                    //FileLoop();

                    //Save Code on Disk


                //}
                //catch { Console.WriteLine("Can't reach " + config.GetSqlCon()); }
            }
            else {

                switch (Function(args))
                {
                    case "types":

                        if (args.Length > 1)
                        {
                            try
                            {
                                Config c = new Config();
                                c.GetTypesTable(c.ReadConfig(), args[1]);
                            }
                            catch (Exception ex) { Console.WriteLine("error: '" +args[1]+ "' did not match any table."); }
                        }
                        else
                        {
                            Config c = new Config();
                            c.GetTypes(c.ReadConfig());
                        }
                        break;
                    case "noargs":
                        Console.WriteLine("Bye!");
                        break;

                }
            }


           //Console.ReadLine();
        }

        private static bool IsGenerateOnly(string[] args)
        {
            if (args.Length == 0) { 
                Console.WriteLine("This is the hix's Class Generator\n Provide \"hix -all\" to interpretate all *.hix files\n Provide \"hix {file name}\" to interpretate specific file");
                return false;
            }
            
            if (args[0] == "generate" && args.Length > 1) return true;
            else return false;
        }

        private static string Function(string[] args)
        {
            return (args.Length == 0)?"noargs":args[0];
        }

        private static bool IsTableNextToRoot(string rootNode)
        {
            string text = rootNode.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            int rootHead = text.IndexOf("[<]");
            int rootTail = text.IndexOf("[>]");

            int tableHead = text.IndexOf("[[table]]");
            int tableTail = text.IndexOf("[[/table]]")+ "[[/table]]".Length;

            bool result = false;
            if ((tableHead - rootHead) == "[<]".Length && rootTail == tableTail) result = true;
            return result;
        }

        public static void CreateFile(string path, string data)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                writer.WriteLine(data);
            }
        }

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
