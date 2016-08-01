using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using System.Diagnostics.PerformanceData;
using System.Diagnostics;

namespace Status
{

    /// <summary>
    /// Polls an IIS app pool as long as the appPool is defined in the web config file.
    /// </summary>
    public class IisAppPool : Resource
    {
        private Server serverInstance;
        private float cpuPercent;
        private int _cpuAlertTreshold;
        private const string _positiveStatusCode = "OK";
        public IisAppPool(string Name, Server server, int cpuAlertTreshold=90)
        {
            this.Name = Name;
            serverInstance = server;
            _cpuAlertTreshold = cpuAlertTreshold;
        }
        public override bool Exist()
        {
            ServerManager server = null;
            bool appPoolExist = false;
            try {
                server = ServerManager.OpenRemote(serverInstance.Name);
                var enumApool = server.ApplicationPools.GetEnumerator();

                while (enumApool.MoveNext())
                {
                    var AppPool = enumApool.Current;
                    if (AppPool.Name.ToLowerInvariant() == this.Name)
                    {
                        appPoolExist = true;
                        break;
                    }


                }
            }
            catch {

                appPoolExist = false;
            }
                
            server.Dispose();
            
            
           
            return appPoolExist;
        }
        public override async Task<State> Poll()
        {
  
            ServerManager server = null;
            ObjectState stateResult = ObjectState.Unknown;
            State state = default(State);
            try
            {
               
               server = ServerManager.OpenRemote(serverInstance.Name) ;
                    if (server != null&& this.Exist())
                    {
                    state = new State();
                    state.Url = string.Concat(this.Name, "@", serverInstance.Name);
                    stateResult = server.ApplicationPools[Name].State;

                    if (stateResult == ObjectState.Started) {

                        if (server.ApplicationPools[Name].WorkerProcesses.Count > 0)
                        {
                            WorkerProcess wp = server.ApplicationPools[Name].WorkerProcesses
                                .Where(p => p
                                .AppPoolName.ToLowerInvariant() == Name.ToLowerInvariant())
                                .FirstOrDefault();

                            PerformanceCounter cpuCounter = new PerformanceCounter("Process"
                                  , "% Processor Time"
                                  , Process.GetProcessById(wp.ProcessId, serverInstance.Name).ProcessName
                                  , serverInstance.Name);

                            cpuPercent = cpuCounter.NextValue();
                            await Task.Delay(2000);
                            cpuPercent = cpuCounter.NextValue();
                            await Task.Delay(30000);


                            //the requests interval time is now 30000
                            int numOfRequests = wp.GetRequests(30000).Count;


                            float cpuPercentAfterThirthySeconds = cpuCounter.NextValue();

                            if (cpuPercent >= _cpuAlertTreshold && cpuPercentAfterThirthySeconds >= _cpuAlertTreshold)
                            {
                                state.Status = "High CPU Load";
                                state.Description =string.Format("The CPU for app pool {0} on {1} has reach {2}% and may cause HTTP 503 errors", Name, serverInstance.Name, cpuPercent);
                                state = GetCommonState(server, state, numOfRequests);
                            }
                            else
                            {
                                state.Status = _positiveStatusCode;

                            }

                            if (!string.IsNullOrEmpty(state.Status))
                            {
                                server.Dispose();
                                cpuCounter.Close();
                                cpuCounter.Dispose();
                            }
                        }
                        else
                        {
                            state.Status = _positiveStatusCode;

                        }

                        return state;
                        



                        } 
                    else
                    {
                        state.Description = stateResult == ObjectState.Started ? string.Format("The app pool on server {0} is in the following in {1} state, the number of worker processes is {2}", serverInstance.Name, stateResult.ToString(), server.ApplicationPools[Name].WorkerProcesses.Count) : "Unavailable";
                        state.Status = stateResult == ObjectState.Started ? _positiveStatusCode: "App Pool Not Started";

                        state = GetCommonState(server, state, 0);
                        if (stateResult == ObjectState.Stopped)
                        {
                            server.ApplicationPools[Name].Start();
                            state.Description += server.ApplicationPools[Name].State == ObjectState.Started ? "\rResolution: The appPool was started" : state.Description;
                        }


                    }


                }
                

            }
            catch(Exception ex)
            {
                state.Status = ex.GetType().Name;
                state.Description = "'polling failed with" + ex.Message;
                state.Url = string.Concat(this.Name, "@", serverInstance.Name);
                return state;
            }
            

            return state;
            
        }

        private State GetCommonState(ServerManager server, State state, int numOfRequests)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(state.Status);
            state.Status = builder.ToString();
            return state;
        }
    }
}
