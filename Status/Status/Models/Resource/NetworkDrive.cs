using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Status
{
    public class NetworkDrive :Resource
    {
        string _networkName;
        NetworkCredential _credentials;
        public NetworkDrive(string networkName)
        {
            this._networkName = networkName;
        }
        public NetworkDrive(string networkName, NetworkCredential credentials)
        {
            this._networkName = networkName;
            this._credentials = credentials;
        }
      
        public string GetAbsoluteUri()
        {
            throw new NotImplementedException();
        }

        public override async Task<State> Poll()
        {
            State state = new State();
            state.Url = _networkName;
            state.Type = "NetworkDrive";
            state.Status = "Unknown";
            await Task.Run(() =>
            {
                return state.Status = TestPath() == false ? "NotAccesible" : "OK";
            });

            return state;
        }
        private bool TestPath()
        {
            return Directory.Exists(_networkName);
        }
    }
}
