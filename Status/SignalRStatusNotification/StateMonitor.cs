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
       
        private IList<State> st = new List<State>();
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
            await Task.Run(()=> BeginPolling());

            
        }

        private  void BeginPolling()
        {

            while (ResourcePoller.IsNetworkAvailable())
            {
                System.Threading.Thread.Sleep(this._interval);
                foreach (var resource in urls)
                {
                       
                    ResourcePoller poller = new ResourcePoller(resource, this._interval);
                    Console.WriteLine("Currently Polling:{0}---{1} ", resource.Name, resource.GetAbsoluteUri());
                    Task<State> pollingTask  = poller.PollAsync();

                    pollingTask.ContinueWith(t =>
                    {
                        StringBuilder errMsgs = new StringBuilder();
                        Console.WriteLine("Currently Polling:{0}---{1} ", resource.Name, resource.GetAbsoluteUri());
                        ProcessTask(t, resource, errMsgs);

                    });


            }

                if (_states.Any())
                {
                    StateLogger.SendAlertNotification(_states, _notificationRecipients);
                    _states.Clear();
                }
                
            }
        }

        private void ProcessTask(Task<State> t, Resource resource, StringBuilder errMsgs)
        {
            switch (t.Status)
            {

                case TaskStatus.RanToCompletion:
                    State state = t.Result;
                    Print(state, TaskStatus.RanToCompletion.ToString());
                    if (state.Status != HttpStatusCode.OK.ToString())
                        _states.Add(state);
                    
                    break;
                case TaskStatus.Canceled:
                    t.Exception.Flatten().InnerExceptions.ToList().ForEach(exception =>
                    {
                        errMsgs.AppendLine(exception.Message + " - " + exception.InnerException);
                        _states.Add(new State() { Url = resource.GetAbsoluteUri(), Status = errMsgs.ToString() });
                    });
                    //Print(resource, TaskStatus.Canceled.ToString().ToUpper());
                    break;
                case TaskStatus.Faulted:
                    Print(resource);
                    t.Exception.Flatten().InnerExceptions.ToList().ForEach(exception =>
                    {
                        errMsgs.AppendLine(exception.Message + " - " + exception.InnerException);
                        _states.Add(new State() { Url = resource.GetAbsoluteUri(), Status = errMsgs.ToString() });
                    });

                    break;
                default:
                    break;
            }
        }

        private static void Print(Resource resource, string taskStatus=null)
        {
            if (taskStatus == null)
                StateLogger.Print(resource, TaskStatus.Faulted.ToString().ToUpper());
            else
                StateLogger.Print(resource, taskStatus);
        }

        private static void Print(State state , string taskStatus)
        {
            StateLogger.Print(state, taskStatus);
        }

        private void RemoveStateFromCollection(Task<State> pollresult)
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
