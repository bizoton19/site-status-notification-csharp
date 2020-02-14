using Status;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using StateMonitor.Datastore;

namespace StateMonitor.Main
{
    /// <summary>
    /// Defines the <see cref="PollUntilOperationCompletes" />
    /// </summary>
    public class PollUntilOperationCompletes
    {
        /// <summary>
        /// Defines the _interval
        /// </summary>
        private static int _interval;

        private static string _connectionString;

        /// <summary>
        /// Defines the _logger
        /// </summary>
        private static StateLogger _logger;

        /// <summary>
        /// The GetMode
        /// </summary>
        /// <param name="mode">The mode<see cref="string"/></param>
        /// <returns>The <see cref="MODE"/></returns>
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
             _connectionString = String.IsNullOrEmpty(ConfigurationManager.AppSettings["connString"]) ? "" : "";
            string runMode = String.IsNullOrEmpty(ConfigurationManager.AppSettings["mode"]) ? "background" : "background";
            var intervalValue = ConfigurationManager.AppSettings["PollingInterval"];
            intervalValue = String.IsNullOrEmpty(intervalValue) ? "20000" : intervalValue;
            _interval = int.Parse(intervalValue);

            _logger = new StateMonitor.Engine.Logger(
                GetMode(runMode), new SqlServerResourceRepository(_connectionString)
               ); ;
            try
            {
                Task.WaitAll(Task.Run(async () => await BeginMonitoring(_interval, GetMode(runMode))));
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
        /// <param name="mode">The mode<see cref="MODE"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task BeginMonitoring(int interval, MODE mode)
        {
            var apps = new List<Application>();
            var _recipients = ConfigurationManager.AppSettings["To"].ToString();
            var _resources = ConfigurationManager.AppSettings["resources"];
            _resources = String.IsNullOrEmpty(_resources) ? "https://www.google.com" : _resources;
            List<Resource> resources = new List<Resource>();

            var urls = _resources.Split(',').ToList();

            urls.ForEach((Action<string>)(s =>
            {
                Status.Resource r = new ResourceFactory().GetResource((string)s);
                if (r.Exist())
                    resources.Add((Resource)r);
                else
                    Console.WriteLine("Unrecognizable resource: {0} . Please verify config file", string.Concat((string)r.Name, "@", (string)r.GetAbsoluteUri()));
            }));

            try
            {
                StateMonitor.Engine.StateMonitor sm = new StateMonitor.Engine.StateMonitor(_interval, resources, mode,new SqlServerResourceRepository(_connectionString));
                await sm.Init();
            }
            catch (System.AggregateException ex)
            {
                Console.WriteLine("Program Crashed because of " + ex.Data);
                IList<State> st = new List<State>();
                st.Add(new State() { Status = ex.StackTrace });
                await _logger.Log(st
                         .Where(s => s.Status != "OK")
                         .ToList<State>(), _recipients);
                st.ToList().ForEach(x =>
                {
                    _logger.Log(x, x.Status);
                });

            }
        }
    }
}
