/*
The following example demonstrates using asynchronous methods to
get Domain Name System information for the specified host computer.
This example polls to detect the end of the asynchronous operation.
*/

using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using SignalRStatusNotification;

namespace Status.AyncPoller
{
    public class PollUntilOperationCompletes
    {
        public static void Main(string[] args){
        //string url = "http://127.0.0.1:8088/";
            //var server = new Server(url);

            // Map the default hub url (/signalr)
           // server.MapHubs();

            // Start the server
           // server.Start();

          //  Console.WriteLine("Server running on {0}", url);

            // Keep going until somebody hits 'x'
            while (true) {
             StateMonitor sm = new StateMonitor(20000, new System.Collections.Generic.List<string> { "http://www.cpsc.gov", "http://www.saferproducts.gov" });
             sm.Init();
                ConsoleKeyInfo ki = Console.ReadKey(true);
                if (ki.Key == ConsoleKey.X) {
                    break;
                }
            }
        }

    }
}