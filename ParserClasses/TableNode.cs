using System;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;

namespace hix
{
    class TableNode
    {
        private List<ColumnNode> columnNodes;

        internal List<ColumnNode> ColumnNodes
        {
            get { return columnNodes; }
            set { columnNodes = value; }
        }
        private string original;

        public string Original
        {
            get { return original; }
            set { original = value; }
        }
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        private string scheme;

        public string Scheme
        {
            get { return scheme; }
            set { scheme = value; }
        }
        private Table table;

        internal Table Table
        {
            get { return table; }
            set { table = value; }
        }

        public void Create(string text)
        {
            ReadOriginal(text);
            ReadContent(original);
            ReadScheme(content);
        }
        public void ReadOriginal(string filename)
        {            
            this.original = filename;
        }

        public void ReadContent(string original)
        {
            this.content = original;
        }

        public void ReadScheme(string content)
        {

            string copyContent = content;
            string OriginalContent = content;
            this.ColumnNodes = new List<ColumnNode>();
            //if (content.IndexOf("[[column]]") >= 0) 
            //{
            //    ColumnNode columnNode = new ColumnNode();
            //    this.ColumnNodes.Add(columnNode);
            //}
            //var op = content.IndexOf("[[column]]");
            while (content.IndexOf("[[column]]") >= 0)
            {

                int head = content.IndexOf("[[column]]") + "[[column]]".Length;
                int tail = content.IndexOf("[[/column]]");

                copyContent = content.Substring(head, tail - head);

                int replaceHead = content.IndexOf("[[column]]");
                int replaceTail = content.IndexOf("[[/column]]") + "[[/column]]".Length;

                content = ReplaceFirst(content, content.Substring(replaceHead, replaceTail - replaceHead), "[[columns]]");
                this.Scheme = content;
                //Console.WriteLine(content);
                ColumnNode columnNode = new ColumnNode();
                columnNode.Create(copyContent, "notype");
                this.ColumnNodes.Add(columnNode);
                //TableNodes.Add(content.Substring(head, content.Length - head));
            }

            //content = content.Replace("[[columns]]","");
            copyContent = content;
            var r = Regex.Match(content, @"\[\[(column )\w+\]\]");
            var re = Regex.Match(content, @"\[\[\/(column )\w+\]\]");
            while (content.IndexOf(re.Groups[0].Value) >= 0)
            {
                
                int head = r.Index + r.Groups[0].Value.Length;
                int tail = re.Index; 

                copyContent = content.Substring(head, tail - head);

                int replaceHead = r.Index;
                int replaceTail = re.Index + re.Groups[0].Value.Length;

                content = ReplaceFirst(content, content.Substring(replaceHead, replaceTail - replaceHead), "[[columns]]");
                this.Scheme = content;
                //Console.WriteLine(content);
                string type = r.Groups[0].Value.Split(' ')[1].Replace("[[", "").Replace("]]", "");

                ColumnNode columnNode = new ColumnNode();
                columnNode.Create(copyContent, type);
                this.ColumnNodes.Add(columnNode);
                //TableNodes.Add(content.Substring(head, content.Length - head));
            }


        }

        public string GetAllColums()
        {
            string result = "";
            foreach (ColumnNode columnNode in this.columnNodes) result += columnNode.Scheme;
            return result;
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
