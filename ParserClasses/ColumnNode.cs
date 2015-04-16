﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aven
{
    class ColumnNode
    {
       
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
        private Column column;

        internal Column Column
        {
            get { return column; }
            set { column = value; }
        }
        private Database db;

        internal Database Db
        {
            get { return db; }
            set { db = value; }
        }

        public void Create(string text)
        {
            this.Original = text;
            this.Content = text.Replace("<<ColumnNode>>", "").Replace("<<EndColumnNode>>", "");
            this.Scheme = content;
        }

        
    }
}
