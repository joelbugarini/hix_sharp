using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aven
{
    public class RootNode
    {
        
        private List<TableNode> tableNodes;

        internal List<TableNode> TableNodes
        {
            get { return tableNodes; }
            set { tableNodes = value; }
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

        public void Create(string filename, Database db)
        {
            ReadOriginal(filename);
            ReadContent(original);
            ReadScheme(content);
        }

        public void ReadOriginal(string filename)
        {
            string text = System.IO.File.ReadAllText(filename + ".aven");
            this.original = text;
        }
        public void ReadContent(string original) 
        {
            int head = original.IndexOf("<<RootNode>>") + "<<RootNode>>".Length;
            int tail = original.IndexOf("<<EndRootNode>>");
            this.content = original.Substring(head, tail - head);
        }

        public void ReadScheme(string content)
        {
            string contentCopy = content;
            this.TableNodes = new List<TableNode>();
            int ss = content.IndexOf("<<TableNode>>");
            while (content.IndexOf("<<TableNode>>") >= 0)
            {
                int head = content.IndexOf("<<TableNode>>") + "<<TableNode>>".Length;
                int tail = content.IndexOf("<<EndTableNode>>");

                contentCopy = content.Substring(head, tail - head);

                int replaceHead = content.IndexOf("<<TableNode>>");
                int replaceTail = content.IndexOf("<<EndTableNode>>") + "<<EndTableNode>>".Length;

                content = content.Replace(content.Substring(replaceHead, replaceTail - replaceHead), "<<Table>>");
                this.Scheme = content;

                TableNode tableNode = new TableNode();
                tableNode.Create(contentCopy);
                this.TableNodes.Add(tableNode);
               
            }
        }

        public string GetAllTables()
        {
            string result = "";
            foreach (TableNode tableNode in this.tableNodes) result += tableNode.Scheme;
            return result;
        }
    }
}
