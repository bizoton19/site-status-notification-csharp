using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
    public class Environment
    {
        private string _Name;
        private string _Description;
        public Environment()
        {

        }

        public Environment(string Name,string Description = "")
        {
            _Name = Name;
            _Description = Description;
        }

        public string Description { get => _Description; set => _Description = value; }
        public string Name { get => _Name; set => _Name = value; }
    }
}


