using System;
using System.Collections.Generic;

using System.Windows.Forms;

using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace hix
{
    class MainConsole
    {
        public static bool a { get; set; } // all
        public static bool f { get; set; } // single file
        public static bool c { get; set; } // save to clippboard
        public static bool p { get; set; } // save with prefix
        public static bool s { get; set; } // save with sufix
        public static bool single { get; set; } // single table

        [STAThread]
        static void Main(string[] args)
        {
            a = true; f = c = p = s = false;

            string[] arguments = ParseArgs(args);
            ParseOpts(args);

            if (arguments.Length <= 0) Exit("hix code generator\n");
            Config cn = new Config();


            switch (arguments[0])
            {
                case "generate": Generate(arguments); break;
                case "tables": cn.GetTablesConsole(cn.ReadConfig()); break;
                case "help": Exit(help.text); break;
                case "man": Exit(help.text); break;
                default: break;
            }            
        }

        [STAThread]
        private static void Generate(string[] arguments)
        {
            if (arguments.Length == 3) single = true;
            //Read Config File
            Config config = new Config().ReadConfig();

            //Get the DB schema
            Console.WriteLine("hix generator is reading from the path " + config.Models);

            //try
            //{
            Schema db = config.GenerateDb(config);

            //Read File

            Console.WriteLine("Reading the " + arguments[1] + " File");
            RootNode rootNode = new RootNode();
            rootNode.Create(arguments[1], db);

            //Map File to Generate Code

            foreach (TableNode tableNode in rootNode.TableNodes)
            {
                var tables = single ? db.Tables.FindAll(x => x.Name == arguments[2]) : db.Tables;
                foreach (Table table in tables)
                {

                    table.Content = tableNode.Scheme;
                    foreach (ColumnNode columnNode in tableNode.ColumnNodes)
                    {
                        string dataColumnNode = "";
                        if (columnNode.Type == "notype")
                        {
                            foreach (Column column in table.Columns)
                            {
                                if (!columnNode.Ignored.Contains(column.Name))
                                {
                                    dataColumnNode += columnNode.Scheme.Replace(
                                        "[[column.name]]", column.Name).Replace(
                                        "[[column.name].[lower]]", LowercaseFirst(column.Name)).Replace(
                                        "[[column.name].[head]]", column.Name.Substring(1)).Replace(
                                        "[[column.type]]", column.Type).Replace(
                                        "[[project.name]]", config.Project).Replace(
                                        "[[database.name]]", config.Project);
                                }
                            }
                        }
                        else
                        {
                            foreach (Column column in table.Columns)
                            {
                                if (columnNode.Type == column.Type)
                                {
                                    if (!columnNode.Ignored.Contains(column.Name))
                                    {
                                        dataColumnNode += columnNode.Scheme.Replace(
                                                "[[column.name]]", column.Name).Replace(
                                                "[[column.name].[lower]]", LowercaseFirst(column.Name)).Replace(
                                                "[[column.name].[head]]", column.Name.Substring(1)).Replace(
                                                "[[column.type]]", column.Type).Replace(
                                                "[[project.name]]", config.Project).Replace(
                                                "[[database.name]]", config.Project);
                                    }
                                }                                
                            }
                        }


                        table.Content = ReplaceFirst(table.Content, "[[columns]]", dataColumnNode).Replace(
                                "[[table.name]]", table.Name).Replace(
                                "[[table.name].[lower]]", LowercaseFirst(table.Name)).Replace(
                                "[[table.name].[init]]", table.Name.Substring(0,table.Name.Length-1)).Replace(
                                "[[project.name]]", config.Project).Replace(
                                "[[database.name]]", config.Project);

                    }
                }

            }

            if (IsTableNextToRoot(rootNode.Original))
            {
                var tables = single ? db.Tables.FindAll(x => x.Name == arguments[2]) : db.Tables;
                foreach (Table table in tables)
                {                    
                    string file = Path.GetFileNameWithoutExtension(arguments[1]);
                    string extension = Path.GetExtension(arguments[1]);
                    (new FileInfo("output/" + file + "/")).Directory.Create();

                    var filepath = "output/" + file + "/" + table.Name + extension;
                    if (s) filepath = "output/" + file + "/" + table.Name + file + extension;
                    if (p) filepath = "output/" + file + "/" + file + table.Name + extension;
                    if (!s && !p) filepath = "output/" + file + "/" + table.Name + extension;

                    if (c){ Clipboard.SetText(table.Content); }
                    else { CreateFile(filepath, table.Content); }

                    
                    Console.WriteLine(filepath + " Created");
                }
            }
            else
            {
                string document = "";
                var tables = single ? db.Tables.FindAll(x => x.Name == arguments[2]) : db.Tables;
                foreach (Table table in tables)
                {
                    document += table.Content;
                }
                document = rootNode.Scheme.Replace("[[table.container]]", document)
                                          .Replace("[[project.name]]", config.Project)
                                          .Replace("[[database.name]]", config.Project);

                string file = Path.GetFileNameWithoutExtension(arguments[1]);
                string extension = Path.GetExtension(arguments[1]);
                (new FileInfo("output/" + file + "/")).Directory.Create();
                if (c) { Clipboard.SetText(document); }
                else { CreateFile("output/" + file + "/" + file + extension, document); }
                Console.WriteLine("output/" + file + "/" + file + extension + " Created");

            }            
        }

        private static void ParseOpts(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg[0] == '-')
                {
                    foreach (char opt in arg)
                    {
                        switch (opt)
                        {
                            case '-': break;
                            case 'a': a = true; break;
                            case 'f': f = true; break;
                            case 'c': c = true; break;
                            case 'p': p = true; break;
                            case 's': s = true; break;
                            default: Exit("'" + opt + "' is not a recognized option, see hix help for details."); break;                       
                        }
                    }
                }
            }
        }

        private static string[] ParseArgs(string[] args)
        {
            var result = new List<string>();
            foreach (string arg in args)
            {
                if (arg[0] != '-') result.Add(arg);
            }

            return result.ToArray();
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

        public static string LowercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToLower(s[0]) + s.Substring(1);
        }

        public static void Exit(string text)
        {
            Console.Write(text);
            Environment.Exit(0);
        }
    }
}
