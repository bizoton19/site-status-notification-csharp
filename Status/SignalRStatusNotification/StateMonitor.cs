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
using System.Net;

namespace SignalRStatusNotification
{
    public class StateMonitor
    {
        private IHubContext _hubs;
        private int _interval;
        private List<Resource> urls;
        private HashSet<State> _states = new HashSet<State>();
        private string _notificationRecipients;

        public StateMonitor(int updateInterval, List<Resource> resourceUris)
        {
            _hubs = GlobalHost.ConnectionManager.GetHubContext<StatusNotificationHub>();
           this._interval = updateInterval;
            urls = resourceUris;
        }
        public async Task Init()
        {
            _notificationRecipients = System.Configuration.ConfigurationManager.AppSettings["To"].ToString();
            await Task.Run(new Action(BeginPolling));
        
                if (_states.Any())
                {
                    StateLogger.SendAlertNotification(_states, _notificationRecipients);
                }
            
        }

        private void BeginPolling()
        {


            while (ResourcePoller.IsNetworkAvailable())
            {
                foreach (var resource in urls)
                {
                    try
                    {

                        if (resource.GetType() == typeof(WindowsService))
                            MonitorWindowsService(resource);
                        if (resource.GetType() == typeof(Server))
                            MonitorServer(resource);
                        if (resource.GetType() == typeof(HttpResource))
                            MonitorHttpResource(resource);
                    }
                    catch (Exception ex)
                    {
                        IList<State> st = new List<State>();
                        st.Add(new State() { Status = ex.Message + "-" + ex.StackTrace });
                        StateLogger.SendAlertNotification(st, _notificationRecipients);
                    }

                }
                if (_states.Any()) { 
                StateLogger.SendAlertNotification(_states, _notificationRecipients);
                }


            }
        }

        private void MonitorWindowsService(Resource resource)
        {
            ResourcePoller poller = new ResourcePoller(resource, this._interval);
            Task<State> pollresult = poller.PollWindowsServiceAsync();

            State state = pollresult.Result;
            Console.WriteLine("Result: " + pollresult.Result.Url + " " + pollresult.Result.Status);
            if (pollresult.Result.Status != "Running" && !_states.Contains(pollresult.Result))
            {
                _states.Add(pollresult.Result);
            }
            else
            {
                RemoveStateFromNotificationList(pollresult);
            }
            
        }
        private void MonitorServer(Resource resource)
        {
            ResourcePoller poller = new ResourcePoller(resource, this._interval);
            Task<State> pollresult = poller.PollServerAsync();

            State state = pollresult.Result;
            Console.WriteLine("Result: " + pollresult.Result.Url + " " + pollresult.Result.Status);

            if (pollresult.Result.Status != "Success" && !_states.Contains(pollresult.Result))
            {
                _states.Add(pollresult.Result);
            }
            else
            {
                RemoveStateFromNotificationList(pollresult);
            }


        }
        private void MonitorHttpResource(Resource resource)
        {
            ResourcePoller poller = new ResourcePoller(resource, this._interval);
            Task<State> pollresult = poller.PollHttpAsync();


            State state = pollresult.Result;
            Console.WriteLine("Result: " + pollresult.Result.Url + " " + pollresult.Result.Status);

            if (pollresult.Result.Status != HttpStatusCode.OK.ToString() && !_states.Contains(pollresult.Result))
            {

                _states.Add(pollresult.Result);

            }
            else
            {
                RemoveStateFromNotificationList(pollresult);
            }
        }

        private void RemoveStateFromNotificationList(Task<State> pollresult)
        {
            if (_states.Contains(pollresult.Result))
            {
                _states.Remove(pollresult.Result);
            }
        }

        // try
        // {
        // await _hubs.Clients.All.broadcastStatus(result.Result.Url, result.Result.Status, DateTime.UtcNow.ToString());
        // }
        // catch (SocketException e)
        // {
        // Console.WriteLine("An exception occurred while processing the request: {0}", e.Message);
        // }




    }
}
