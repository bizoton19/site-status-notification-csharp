using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public struct State
    {
        public string Url { get; set; }
        public string Status { get; set; }
        public int ResponseTime { get; set; }
        public string Description { get; set;   }

        public string Type { get; set; }

       
    }
}
