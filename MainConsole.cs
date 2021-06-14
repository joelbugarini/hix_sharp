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
        public static bool single { get; set; } // single model

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
                case "init": Initialize(arguments); break;
                case "models": cn.GetModelsConsole(cn.ReadConfig()); break;
                case "help": Exit(help.text); break;
                case "man": Exit(help.text); break;
                default: break;
            }            
        }

        private static void Initialize(string[] arguments)
        {
           // TODO: Implement project initializer
        }

        [STAThread]
        private static void Generate(string[] arguments)
        {
            if (arguments.Length == 3) single = true;
            //Read Config File
            Config config = new Config().ReadConfig();

            //Get the DB schema
            Console.WriteLine("hix generator is reading models from " + config.Models + " folder");

            //try
            //{
            Schema schema = config.GenerateSchema(config);

            //Read File

            Console.WriteLine("Reading the " + arguments[1] + " File");
            RootNode rootNode = new RootNode();
            rootNode.Create(arguments[1], schema);

            //Map File to Generate Code

            foreach (ModelNode modelNode in rootNode.ModelNodes)
            {
                var models = single ? schema.Models.FindAll(x => x.Name == arguments[2]) : schema.Models;
                foreach (Model model in models)
                {

                    model.Content = modelNode.Scheme;
                    foreach (PropNode propNode in modelNode.PropNodes)
                    {
                        string dataPropNode = "";
                        if (propNode.Type == "notype")
                        {
                            foreach (Property prop in model.Props)
                            {
                                if (!propNode.Ignored.Contains(prop.Name))
                                {
                                    dataPropNode += propNode.Scheme.Replace(
                                        "[[prop.name]]", prop.Name).Replace(
                                        "[[prop.name].[lower]]", LowercaseFirst(prop.Name)).Replace(
                                        "[[prop.type]]", prop.Type).Replace(
                                        "[[project.name]]", config.Project).Replace(
                                        "[[database.name]]", config.Project);
                                }
                            }
                        }
                        else
                        {
                            foreach (Property prop in model.Props)
                            {
                                if (propNode.Type == prop.Type)
                                {
                                    if (!propNode.Ignored.Contains(prop.Name))
                                    {
                                        dataPropNode += propNode.Scheme.Replace(
                                                "[[prop.name]]", prop.Name).Replace(
                                                "[[prop.name].[lower]]", LowercaseFirst(prop.Name)).Replace(
                                                "[[prop.type]]", prop.Type).Replace(
                                                "[[project.name]]", config.Project).Replace(
                                                "[[database.name]]", config.Project);
                                    }
                                }                                
                            }
                        }


                        model.Content = ReplaceFirst(model.Content, "[[props]]", dataPropNode).Replace(
                                "[[model.name]]", model.Name).Replace(
                                "[[model.name].[lower]]", LowercaseFirst(model.Name)).Replace(
                                "[[model.name].[init]]", model.Name.Substring(0,model.Name.Length-1)).Replace(
                                "[[project.name]]", config.Project).Replace(
                                "[[database.name]]", config.Project);

                    }
                }

            }

            if (IsTableNextToRoot(rootNode.Original))
            {
                var models = single ? schema.Models.FindAll(x => x.Name == arguments[2]) : schema.Models;
                foreach (Model model in models)
                {                    
                    string file = Path.GetFileNameWithoutExtension(arguments[1]);
                    string extension = Path.GetExtension(arguments[1]);
                    (new FileInfo("output/" + file + "/")).Directory.Create();

                    var filepath = "output/" + file + "/" + model.Name + extension;
                    if (s) filepath = "output/" + file + "/" + model.Name + file + extension;
                    if (p) filepath = "output/" + file + "/" + file + model.Name + extension;
                    if (!s && !p) filepath = "output/" + file + "/" + model.Name + extension;

                    if (c){ Clipboard.SetText(model.Content); }
                    else { CreateFile(filepath, model.Content); }

                    
                    Console.WriteLine(filepath + " Created");
                }
            }
            else
            {
                string document = "";
                var models = single ? schema.Models.FindAll(x => x.Name == arguments[2]) : schema.Models;
                foreach (Model model in models)
                {
                    document += model.Content;
                }
                document = rootNode.Scheme.Replace("[[model.container]]", document)
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

            int modelHead = text.IndexOf("[[model]]");
            int modelTail = text.IndexOf("[[/model]]")+ "[[/model]]".Length;

            bool result = false;
            if ((modelHead - rootHead) == "[<]".Length && rootTail == modelTail) result = true;
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
            Console.WriteLine(text);
            Environment.Exit(0);
        }
    }
}
