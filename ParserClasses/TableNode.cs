using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aven
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
            this.ColumnNodes = new List<ColumnNode>();
            while (content.IndexOf("<<ColumnNode>>") >= 0)
            {
                int head = content.IndexOf("<<ColumnNode>>") + "<<ColumnNode>>".Length;
                int tail = content.IndexOf("<<EndColumnNode>>");

                copyContent = content.Substring(head, tail - head);

                int replaceHead = content.IndexOf("<<ColumnNode>>");
                int replaceTail = content.IndexOf("<<EndColumnNode>>") + "<<EndColumnNode>>".Length;

                content = content.Replace(content.Substring(replaceHead, replaceTail - replaceHead), "<<Columns>>");
                this.Scheme = content;

                ColumnNode columnNode = new ColumnNode();
                columnNode.Create(copyContent);
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

    }
}
