using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public  abstract class Resource : IPollable
    {
        
        public string Status { get; private set; }
        public string Name { get; private set; }
        internal string Url{ get; set; }
        public virtual Task<dynamic> Poll()
        {
           throw new  NotImplementedException();
        }

        public virtual  bool Exist()
        {
            throw new NotImplementedException();
        }

        public virtual int GetErrorCount()
        {
            throw new NotImplementedException();
        }

        public virtual string GetAbsoluteUri()
        {
            return this.Url;
        }
    }
}
