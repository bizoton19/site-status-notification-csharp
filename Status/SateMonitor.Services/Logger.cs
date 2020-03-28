
using Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StateMonitor.Engine
{
    public class Logger : StateLogger
    {
        public Logger(MODE mode, IResourceRepository repo) : base(mode,repo)
        {
        }
         
    }
}