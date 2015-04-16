using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace aven
{
    class Program
    {
        static void Main(string[] args)
        {
            //Validate args
            if (args.Length > 0)
            {
                //Read Config File
                Config config = new Config().ReadConfig();

                //Get the DB schema
                Console.WriteLine("aven Class Generator is connecting to Database " + config.Server + "\\" + config.Database);
                Database db = config.GenerateDb(config);

                //Read File
                Console.WriteLine("Reading the " + args[0] + " File");
                RootNode rootNode = new RootNode();
                rootNode.Create(args[0], db);

                //Map File to Generate Code

                foreach (TableNode tableNode in rootNode.TableNodes)
                {
                    foreach (Table table in db.Tables)
                    {
                        table.Content = tableNode.Scheme;
                        foreach (ColumnNode columnNode in tableNode.ColumnNodes)
                        {
                            string dataColumnNode = "";
                            foreach (Column column in table.Columns)
                            {
                                dataColumnNode += columnNode.Scheme.Replace(
                                    "<<Column.Name>>", column.Name).Replace(
                                    "<<Column.Type>>", column.Type).Replace(
                                    "<<Project.Name>>", config.Project).Replace(
                                    "<<Database.Name>>", config.Database);
                            }

                           

                            table.Content = ReplaceFirst(table.Content, "<<Columns>>", dataColumnNode).Replace(
                                "<<Table.Name>>", table.Name).Replace(
                                "<<Project.Name>>", config.Project).Replace(
                                "<<Database.Name>>", config.Database);

                        }
                    }

                }

                foreach (Table table in db.Tables)
                {
                    string file = Path.GetFileNameWithoutExtension(args[0]);
                    string extension = Path.GetExtension(args[0]);
                    (new FileInfo(file + "/")).Directory.Create();
                    CreateFile(file + "/" + file + table.Name + extension, table.Content);
                    Console.WriteLine(file + "/" + file + table.Name + extension + " Created");
                }

                //FileLoop();

                //Save Code on Disk

                Console.ReadLine();
            }
            else { Console.WriteLine("This is the aven's Class Generator\n Provide \"aven -all\" to interpretate all *.aven files\n Provide \"aven {file name}\" to interpretate specific file"); }
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
