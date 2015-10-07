using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hoob
{
    class Table
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private List<Column> columns;

        internal List<Column> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }
    }
}
