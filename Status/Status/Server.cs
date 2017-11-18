   


  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;

namespace Status
{
  public  class Server : Resource
    {
        private State _state;
        private string _serverName;
        private string _ipAddress;
        private const int pingTimeout = 10000;
        private const int roundTripTime = 3000;

        public string ServerName
        {
            get
            {
                return _serverName;
            }

            set
            {
                _serverName = value;
            }
        }

        public string IpAddress
        {
            get
            {
                return _ipAddress;
            }

            set
            {
                _ipAddress = value;
            }
        }
        
        public  Server(string serverName=null, string ipAddress=null)
        {
            Name = serverName;
            this.Url = serverName;
            ServerName = serverName;
            IpAddress = ipAddress;
        }
        public override async Task<State> Poll()
        {
            var ping = new Ping();
            var data = "B".PadRight(32, 'B');
            var buffer = Encoding.ASCII.GetBytes(data);
            PingReply reply = await ping.SendPingAsync(ServerName, pingTimeout , buffer, new PingOptions(128,true));
            
            _state = new State();
            _state.Url = string.Concat(this.Name, "-", reply.Address.ToString());
            _state.Status = reply.Status == IPStatus.Success ? "OK" : GetPingMessage(reply);
            return _state;
        }

        private string GetPingMessage(PingReply reply)
        {
           string error = "";
           string timoutError = reply.Status != IPStatus.Success ? 
                $"The timout Time period of {pingTimeout} has been exceeded"
                : $"Roundtrip time { reply.RoundtripTime.ToString()}";

            string roundTripError = reply.RoundtripTime > 3000 ?
                $"The roundtrip time period of {roundTripTime * 60} has been exceeded"
                : $"Roundtrip time{reply.RoundtripTime.ToString()} " ;

            if (reply.Status != IPStatus.Success )
                error = timoutError;
            if (reply.RoundtripTime > roundTripTime)
                error = roundTripError;

                return error;
        }
     
    }
}
