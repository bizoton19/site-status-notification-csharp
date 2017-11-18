using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Status
{
    public class WindowsServer: Server
    {
        public WindowsServer(string serverName = null, string ipAddress = null) : base(serverName, ipAddress)
        {

        }
        private ICollection<EventLog> _eventLogs;

        public  ICollection<EventLog> GetEventLogsFor(string[] eventSources)
    {
        _eventLogs = new List<System.Diagnostics.EventLog>();
        eventSources.ToList().ForEach(s =>
        {
            var logName = EventLog.LogNameFromSourceName(s, ServerName);
            if (EventLog.SourceExists(logName))
            {
                EventLog log = CreateEventLog(logName);
                _eventLogs.Add(log);
            }
        });
        return _eventLogs;

    }

    private static EventLog CreateEventLog(string logName)
    {
        return new EventLog(logName);
    }

    }
}
