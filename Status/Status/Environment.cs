using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
    public class Environment
    {
        private List<Resource> _resources = new List<Resource>();

        public void AddResource(Resource resource)
        {
            _resources.Add(resource);
        }

        public IList<Resource> GetResources()
        {
            return _resources;
        }
    }
}


