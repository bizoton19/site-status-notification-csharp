using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status.Events
{
   public class ResourceStateChanged : IApplicationEvent
    {
        public State State { get; set; }
    }
}
