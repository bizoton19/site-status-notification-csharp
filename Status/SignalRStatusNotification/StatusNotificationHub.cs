using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Status;
namespace SignalRStatusNotification
{
    public class StatusNotificationHub : Hub
    {
        public void Send(string url, string status, string datetime)
        {
            Clients.All.broadcastStatus(url, status, datetime);
        }
    }
}