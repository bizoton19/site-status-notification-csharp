using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public interface IPollable
    {
       Task<dynamic> Poll();
       bool Exist();
       string GetAbsoluteUri();
       int GetErrorCount();
    }
}
