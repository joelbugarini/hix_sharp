using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace hix
{
    class ModelNode
    {
        private List<PropNode> propNodes;

        internal List<PropNode> PropNodes
        {
            get { return propNodes; }
            set { propNodes = value; }
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
        private Model model;

        internal Model Table
        {
            get { return model; }
            set { model = value; }
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
            this.PropNodes = new List<PropNode>();
            //if (content.IndexOf("[[prop]]") >= 0) 
            //{
            //    ColumnNode propNode = new ColumnNode();
            //    this.ColumnNodes.Add(propNode);
            //}
            //var op = content.IndexOf("[[prop]]");
            //////while (content.IndexOf("[[prop]]") >= 0)
            //////{

            //////    int head = content.IndexOf("[[prop]]") + "[[prop]]".Length;
            //////    int tail = content.IndexOf("[[/prop]]");

            //////    copyContent = content.Substring(head, tail - head);

            //////    int replaceHead = content.IndexOf("[[prop]]");
            //////    int replaceTail = content.IndexOf("[[/prop]]") + "[[/prop]]".Length;

            //////    content = ReplaceFirst(content, content.Substring(replaceHead, replaceTail - replaceHead), "[[props]]");
            //////    this.Scheme = content;
            //////    //Console.WriteLine(content);
            //////    ColumnNode propNode = new ColumnNode();
            //////    propNode.Create(copyContent, "notype");
            //////    this.ColumnNodes.Add(propNode);
            //////    //TableNodes.Add(content.Substring(head, content.Length - head));
            //////}

            //content = content.Replace("[[props]]","");
            copyContent = content;
            string originalContent = content;
            
            foreach (Match r in Regex.Matches(originalContent, @"(\[\[(prop )\w+\](.\[ignore=[a-zA-Z,_0-9]+\])?\]|\[\[(prop)\](.\[ignore=[a-zA-Z,_0-9]+\])?\])"))
            {
                List<string> ignoreds = GetIgnoreds(r.Value);
                var match = r.Value;
                if (r.Value.Split(' ').Length > 1)
                {
                    string col = Regex.Match(r.Value, @"( )\w+\](.\[ignore=[a-zA-Z,_0]+\]\.)?").Value;
                    string type = TextBetween(col, " ", "]");

                    var re = Regex.Matches(originalContent, @"\[\[\/(prop " + type + @")+\](.\[ignore=[a-zA-Z,_0-9]+\])?\]");
                    int head = r.Index + r.Value.Length;
                    int tail = re[0].Index;

                    int ct = 0;
                    while (tail - head < 0)
                    {
                        ct++;
                        tail = re[ct].Index;
                    }

                    copyContent = originalContent.Substring(head, tail - head);

                    int replaceHead = r.Index;
                    int replaceTail = re[ct].Index + re[ct].Value.Length;

                    content = ReplaceFirst(content, originalContent.Substring(replaceHead, replaceTail - replaceHead), "[[props]]");
                    this.Scheme = content;

                    PropNode propNode = new PropNode();
                    propNode.Create(copyContent, type, ignoreds);
                    this.PropNodes.Add(propNode);
                }
                else {
                    //\[\[(prop)\](.\[ignore=[a-zA-Z,_0-9]+\])?\]
                    string propToken = Regex.Match(content, @"\[\[(prop)\](.\[ignore=[a-zA-Z,_0-9]+\])?\]").Value;
                    int head = content.IndexOf(propToken) + propToken.Length;
                    int tail = content.IndexOf("[[/prop]]");

                    copyContent = content.Substring(head, tail - head);

                    int replaceHead = content.IndexOf(propToken);
                    int replaceTail = content.IndexOf("[[/prop]]") + "[[/prop]]".Length;

                    content = ReplaceFirst(content, content.Substring(replaceHead, replaceTail - replaceHead), "[[props]]");
                    this.Scheme = content;
                    //Console.WriteLine(content);
                    PropNode propNode = new PropNode();
                    propNode.Create(copyContent, "notype", ignoreds);
                    this.PropNodes.Add(propNode);
                    //TableNodes.Add(content.Substring(head, content.Length - head));

                }
            }
        }

        public bool HasIgnored(string text)
        {
            return text.Contains("ignore=");
        }

        public List<string> GetIgnoreds(string text)
        {
            if (HasIgnored(text))
            {
                return TextBetween(text,"ignore=","]]").Split(',').ToList();
            }
            else {
                return new List<string>();
            }            
        }

        public string TextBetween(string St, string from, string to) {
            int pFrom = St.IndexOf(from) + from.Length;
            int pTo = St.LastIndexOf(to);

            return St.Substring(pFrom, pTo - pFrom);
        }

        public string GetAllColums()
        {
            string result = "";
            foreach (PropNode propNode in this.propNodes) result += propNode.Scheme;
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
