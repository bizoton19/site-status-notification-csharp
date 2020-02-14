using StateMonitor.Datastore;
using Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRStatusNotification
{
    public class Logger : StateLogger
    {
        public Logger(MODE mode) : base(mode)
        {
        }
        public Logger(MODE mode,IStateRepository repo) : base(mode,repo)
        {
        }

        
    }
}