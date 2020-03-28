using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public  abstract class Resource : IPollable
    {
        
        
        public string Name { get; protected set; }
        public string Description  { get; set; }
        public string Url{ get; set; }
        public string Type { get; set; }
        public string TypeDescription { get; set; }
        public ResourceId ResourceId { get; set; }

        public abstract Task<State> Poll();
        public virtual bool Exist()
        {
            return !String.IsNullOrEmpty(Url);
        }
        public virtual string GetAbsoluteUri()
        {
            return this.Url;
        }
    }
}
