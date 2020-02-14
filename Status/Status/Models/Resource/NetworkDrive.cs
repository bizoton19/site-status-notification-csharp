using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.OI;
using System.Net;
using System.IO;

namespace Status.Models.Resource
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
            //var state = new State();
            // state.Status = await TestPath() == false ? "NotAccesible" : "OK");

           
        }
        private bool TestPath()
        {
            return Directory.Exists(_networkName);
        }
    }
}
