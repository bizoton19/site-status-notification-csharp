using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Hosting;
using SignalRStatusNotification;
using Status;
using System.Net.Sockets;

namespace SignalRStatusNotification
{
    public class StateMonitor
    {
        private IHubContext _hubs;
        private int _interval;
        private List<string> urls;
        public StateMonitor(int updateInterval, List<string> resourceUris)
        {
            _hubs = GlobalHost.ConnectionManager.GetHubContext<StatusNotificationHub>();
           this._interval = updateInterval;
           this.urls = resourceUris;
        }
        public async Task Init()
        {

            List<Resource> resources = new List<Resource>();
            
            urls.ForEach(s => resources.Add(new HttpResource(new Uri(s)))); 
           
            while (ResourcePoller.IsNetworkAvailable())
            {
              foreach(var r in resources)
              {
                  await BeginStateMonitor(r);
              }

            }
        }

        private async Task BeginStateMonitor(Resource resource)
        {
            ResourcePoller poller = new ResourcePoller(resource, this._interval);

            System.Threading.Tasks.Task<State> result = poller.PollAsync();
            // _hubs.Clients.All.broadcast"Polling resource with internal id of : {0} in {1} ms intervals...", resources.IndexOf(resource), poller.PollingInterval);

            if (result.IsCompleted)
                Console.WriteLine("Result: " + result.Result.Url + " " + result.Result.Status);
                try
                {
                    await _hubs.Clients.All.broadcastStatus(result.Result.Url, result.Result.Status, DateTime.UtcNow.ToString());
                }
                catch (SocketException e)
                {
                    // Console.WriteLine("An exception occurred while processing the request: {0}", e.Message);
                }
        }

        
        
    }
}
