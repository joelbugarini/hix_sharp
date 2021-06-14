using System;
using System.Collections.Generic;

using System.Text;


namespace hix
{
    class Model
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private List<Property> columns;

        internal List<Property> Props
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
