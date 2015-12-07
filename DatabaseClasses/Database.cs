using System;
using System.Collections.Generic;

using System.Text;


namespace hix
{
    public class Database
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private List<Table> tables;

        internal List<Table> Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }
    }
}
