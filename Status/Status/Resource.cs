using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public  abstract class Resource : IPollable
    {
        
        public string Status { get; protected set; }
        public string Name { get; protected set; }
        internal string Url{ get; set; }


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
