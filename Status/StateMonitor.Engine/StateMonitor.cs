namespace StateMonitor.Engine

    using Status;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="StateMonitor" />
    /// </summary>
    public class StateMonitor
    {
        /// <summary>
        /// Defines the _logger
        /// </summary>
        private StateLogger _logger;


        /// <summary>
        /// Defines the _interval
        /// </summary>
        private int _interval;

        /// <summary>
        /// Defines the _mode
        /// </summary>
        private MODE _mode;

        /// <summary>
        /// Defines the urls
        /// </summary>
        private List<Resource> urls;

        /// <summary>
        /// Defines the _states
        /// </summary>
        private HashSet<State> _states = new HashSet<State>();

        /// <summary>
        /// Defines the st
        /// </summary>
        private IList<State> st = new List<State>();

        /// <summary>
        /// Defines the _notificationRecipients
        /// </summary>
        private string _notificationRecipients;
        private IResourceRepository _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMonitor"/> class.
        /// </summary>
        /// <param name="updateInterval">The <see cref="int"/></param>
        /// <param name="resourceUris">The <see cref="List{Resource}"/></param>
        public StateMonitor(int updateInterval, List<Resource> resourceUris, MODE mode, IResourceRepository state)
        {
            _interval = updateInterval;
            urls = resourceUris;
            _mode = mode;
            _state = state;
        }

        /// <summary>
        /// The Init
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task Init()
        {
            _notificationRecipients = System.Configuration.ConfigurationManager.AppSettings["To"].ToString();
           
            _logger = new Logger(
                (_mode == MODE.Console)
                ? MODE.Console
                : MODE.Background,
                _state
             );
            await Task.Run(() => BeginPolling());
        }

        /// <summary>
        /// The BeginPolling
        /// </summary>
        private void BeginPolling()
        {
                foreach (var resource in urls)
                {
                    ResourcePoller poller = new ResourcePoller(resource, this._interval);
                    Console.WriteLine("Currently Polling:{0}---{1} ", resource.Name, resource.GetAbsoluteUri());
                    Task<State> pollingTask = poller.PollAsync();
                    pollingTask.ContinueWith(t =>
                    {
                        StringBuilder errMsgs = new StringBuilder();
                        Console.WriteLine("Currently Polling:{0}---{1} ", resource.Name, resource.GetAbsoluteUri());
                        ProcessTask(t, resource, errMsgs);
                    });
                }

                if (_states.Any())
                {

                //map _states list to another list for just states that are in error then call the alert logger
                Task<StateLogger> logResults =  _logger.Log( _states
                          .Where(s => s.Status != "OK")
                          .ToList<State>(), _notificationRecipients);
                logResults.ContinueWith(l =>
                {
                    _states.ToList().ForEach(x =>
                    {
                        _logger.Log(x, x.Status);
                    });
                });             
                   //_states.Clear();
                }
            
        }

        /// <summary>
        /// The ProcessTask
        /// </summary>
        /// <param name="t">The <see cref="Task{State}"/></param>
        /// <param name="resource">The <see cref="Resource"/></param>
        /// <param name="errMsgs">The <see cref="StringBuilder"/></param>
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
                        _states.Add(new State() { Type=resource.GetType().Name,Url = resource.GetAbsoluteUri(), Status = errMsgs.Length > 0 ? errMsgs.ToString() : "No Exceptions Captured" });
                    });

                    break;

                case TaskStatus.Faulted:
                    Print(resource);
                    t.Exception.Flatten().InnerExceptions.ToList().ForEach(exception =>
                    {
                        errMsgs.AppendLine(exception.Message + " - " + exception.InnerException);
                        _states.Add(new State() { Type = resource.GetType().Name, Url = resource.GetAbsoluteUri(), Status = errMsgs.ToString() });
                    });

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// The Print
        /// </summary>
        /// <param name="resource">The <see cref="Resource"/></param>
        /// <param name="taskStatus">The <see cref="string"/></param>
        private void Print(Resource resource, string taskStatus = null)
        {
            if (taskStatus == null)
                _logger.Log(resource, TaskStatus.Faulted.ToString().ToUpper());
            else
                _logger.Log(resource, taskStatus);
        }

        /// <summary>
        /// The Print
        /// </summary>
        /// <param name="state">The <see cref="State"/></param>
        /// <param name="taskStatus">The <see cref="string"/></param>
        private void Print(State state, string taskStatus)
        {
            _logger.Log(state, taskStatus);
        }

        /// <summary>
        /// The RemoveStateFromCollection
        /// </summary>
        /// <param name="pollresult">The <see cref="Task{State}"/></param>
        private void RemoveStateFromCollection(Task<State> pollresult)
        {
            if (_states.Contains(pollresult.Result))
            {
                _states.Remove(pollresult.Result);
            }
        }
    }
}
