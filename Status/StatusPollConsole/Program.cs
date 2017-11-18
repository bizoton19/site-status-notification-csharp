/*
The following example demonstrates using asynchronous methods to
get Domain Name System information for the specified host computer.
This example polls to detect the end of the asynchronous operation.
*/

namespace Status.AysncPoller
{
    using SignalRStatusNotification;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="PollUntilOperationCompletes" />
    /// </summary>
    public class PollUntilOperationCompletes
    {
        /// <summary>
        /// Defines the _interval
        /// </summary>
        private static int _interval;

        /// <summary>
        /// Defines the _logger
        /// </summary>
        private static StateLogger _logger;
        private static MODE GetMode(string mode)
        {
            return mode == MODE.Console.ToString()
                  ? MODE.Console
                  : MODE.Background;
        }
        /// <summary>
        /// The Main
        /// </summary>
        /// <param name="args">The <see cref="string[]"/></param>
        public static void Main(string[] args)
        {
            string runMode = String.IsNullOrEmpty(ConfigurationManager.AppSettings["mode"]) ? "console" : "background";
            var intervalValue = ConfigurationManager.AppSettings["PollingInterval"];
            intervalValue = String.IsNullOrEmpty(intervalValue) ? "20000" : intervalValue;
            _interval = int.Parse(intervalValue);

            _logger = new Logger(
                GetMode(runMode)
               );
            try
            {
                Task.WaitAll(Task.Run(async () => await BeginMonitoring(_interval,GetMode(runMode))));
            }
            catch (System.AggregateException ex)
            {
                ex.Flatten().InnerExceptions.ToList().ForEach(exception =>
                    {
                        Console.WriteLine(ex.Message + " - " + ex.InnerException);
                        Console.ReadLine();

                    });

            }
        }

        /// <summary>
        /// The BeginMonitoring
        /// </summary>
        /// <param name="interval">The <see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task BeginMonitoring(int interval,MODE mode)
        {
            var _recipients = ConfigurationManager.AppSettings["To"].ToString();
            var _resources = ConfigurationManager.AppSettings["resources"];
            _resources = String.IsNullOrEmpty(_resources) ? "https://www.google.com" : _resources;
            List<Resource> resources = new List<Resource>();

            var urls = _resources.Split(',').ToList();

            urls.ForEach(s =>
            {
                Resource r = new ResourceFactory().GetResource(s);
                if (r.Exist())
                    resources.Add(r);
                else
                    Console.WriteLine("Unrecognizable resource: {0} . Please verify config file", string.Concat(r.Name, "@", r.GetAbsoluteUri()));
            });

            try
            {
                StateMonitor sm = new StateMonitor(_interval, resources,mode);
                await sm.Init();
            }
            catch (System.AggregateException ex)
            {
                Console.WriteLine("Program Crashed because of " + ex.Data);
                IList<State> st = new List<State>();
                st.Add(new State() { Status = ex.StackTrace });

                st.ToList().ForEach(x =>
                {
                    _logger.Log(x, x.Status);
                });

            }
        }
    }
}
