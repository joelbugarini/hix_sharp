using System;
using System.Collections.Generic;

using System.Text;


namespace hix
{
    public class Schema
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private List<Model> models;

        internal List<Model> Models
        {
            get { return models; }
            set { models = value; }
        }
    }
}
