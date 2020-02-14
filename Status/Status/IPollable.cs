using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public interface Resource
    {
        Task<State> Poll();
        string GetAbsoluteUri();
       
    }
}
