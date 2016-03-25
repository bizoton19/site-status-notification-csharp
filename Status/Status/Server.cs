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
            PingReply reply =   await ping.SendPingAsync(ServerName,10); // 1 minute time out (in ms)     
            _state = new State();
            _state.Url = string.Concat(this.Name, "-", reply.Address.ToString());
            _state.Status = reply.Status == IPStatus.Success? "OK": reply.Status.ToString();
            return _state;
        }
     
    }
}
