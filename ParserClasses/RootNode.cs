using System;
using System.Collections.Generic;

using System.Text;


namespace hix
{
    public class RootNode
    {
        
        private List<ModelNode> modelNodes;

        internal List<ModelNode> ModelNodes
        {
            get { return modelNodes; }
            set { modelNodes = value; }
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

        public void Create(string filename, Schema db)
        {
            ReadOriginal(filename);
            ReadContent(original);
            ReadScheme(content);
        }

        public void ReadOriginal(string filename)
        {
            string text = System.IO.File.ReadAllText(filename + ".hix");
            this.original = text;
        }
        public void ReadContent(string original) 
        {
            int head = original.IndexOf("[<]") + "[<]".Length;
            int tail = original.IndexOf("[>]");
            this.content = original.Substring(head, tail - head);
        }

        public void ReadScheme(string content)
        {
            string contentCopy = content;
            this.ModelNodes = new List<ModelNode>();
            int ss = content.IndexOf("[[model]]");
            while (content.IndexOf("[[model]]") >= 0)
            {
                int head = content.IndexOf("[[model]]") + "[[model]]".Length;
                int tail = content.IndexOf("[[/model]]");

                contentCopy = content.Substring(head, tail - head);

                int replaceHead = content.IndexOf("[[model]]");
                int replaceTail = content.IndexOf("[[/model]]") + "[[/model]]".Length;

                content = content.Replace(content.Substring(replaceHead, replaceTail - replaceHead), "[[model.container]]");
                this.Scheme = content;

                ModelNode modelNode = new ModelNode();
                modelNode.Create(contentCopy);
                this.ModelNodes.Add(modelNode);
               
            }
        }

        public string GetAllTables()
        {
            string result = "";
            foreach (ModelNode modelNode in this.modelNodes) result += modelNode.Scheme;
            return result;
        }
    }
}
