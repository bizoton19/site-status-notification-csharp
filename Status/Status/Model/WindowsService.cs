using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ServiceProcess;

namespace Status
{
  public  class WindowsService: Resource
    {
        private  ServiceController winser;
        private  Resource _server;
        private string _serviceName;
        private State _state;
        private const string _unableToRestartMessage = "Service stoped and cannot be restarted";
        private const string _stopedButRestartedMessage = "Service was restarted because it was stoped";
        public WindowsService(string serviceName=null, Resource server= null)
        {
            Name = serviceName;
            ServiceName = serviceName;
            Server = server;
     
        }
        public void Close()
        {
            winser.Close();
        }
        private void InitializeService()
        {
           
            winser = new ServiceController(_serviceName, _server.Name);
        }
        public override async Task<State> Poll()
        {
            bool running = await Task.Run(() => {
                 return IsRunning();
            });

            _state = new State() { Url = string.Concat(this.ServiceName, "@", this.Server.Name) };
            if (!running)
            {
                this.Restart();
                _state.Status = IsRunning()? _stopedButRestartedMessage :  _unableToRestartMessage ;
               
            }
            else
            {
                _state.Status = "OK";
            }

            return _state;
        }
        public bool IsRunning()
        {
            InitializeService();
            var running = this.winser.Status == ServiceControllerStatus.Running ? true : false;
            Close();
            return running;

        }

        public void Restart()
        {
            if (IsRunning())
            {
                InitializeService();
                winser.Refresh();
                winser.Stop();
                winser.WaitForStatus(ServiceControllerStatus.Stopped);
                winser.Start();
                winser.WaitForStatus(ServiceControllerStatus.Running);
            }
            else
            {
                winser.Start();
                winser.WaitForStatus(ServiceControllerStatus.Running);
            }
            Close();
        }

       

        public ServiceController Winser
        {
            get
            {
                return winser;
            }

           
        }

        public Resource Server
        {
            get
            {
                return _server;
            }

            set
            {
                _server = value;
            }
        }

        public string ServiceName
        {
            get
            {
                return _serviceName;
            }

            set
            {
                _serviceName = value;
            }
        }
    }
}
