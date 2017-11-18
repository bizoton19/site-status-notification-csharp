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

        public override StateLogger Save(Resource resource, string taskStatus)
        {
            throw new NotImplementedException();
        }
    }
}