/*
The following example demonstrates using asynchronous methods to
get Domain Name System information for the specified host computer.
This example polls to detect the end of the asynchronous operation.
*/

using System;
using System.Threading.Tasks;
using SignalRStatusNotification;
using System.Linq;
using System.Collections.Generic;

namespace Status.AysncPoller
{
    public class PollUntilOperationCompletes
    {
        private static int _interval; 

        public  static void Main(string[] args){
            _interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PollingInterval"].ToString());
            Task.WaitAll(Task.Run(async () => await BeginMonitoring(_interval)));
          
        }

        public static async Task BeginMonitoring(int interval)
        {
           var _recipients =  System.Configuration.ConfigurationManager.AppSettings["To"].ToString();
            List<Resource> resources = new List<Resource>();
            var urls = System.Configuration.ConfigurationManager.AppSettings["resources"].Split(',').ToList();

            urls.ForEach(s =>
            {
                Resource r = new ResourceFactory().GetResource(s);
                if (r != null)
                    resources.Add(new ResourceFactory().GetResource(s));
                else
                    Console.WriteLine("Unrecognizable resource: {0} . Please verify config file", r.Name);
            });
  
            try {
                StateMonitor sm = new StateMonitor(_interval,resources);
                await sm.Init();
            }
            catch(System.AggregateException ex)
            {
                Console.WriteLine("Program Crashed because of " + ex.Data);
                IList<State> st = new List<State>();
                st.Add(new State() { Status = ex.StackTrace });
                StateLogger.SendAlertNotification(st, _recipients);
            }
            


        }

      


    }

    
}