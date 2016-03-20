using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using Status;

[assembly: OwinStartup(typeof(SignalRStatusNotification.Startup))]

namespace SignalRStatusNotification
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            List<Resource> resources = new List<Resource>();
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
            System.Configuration.ConfigurationManager
                .AppSettings["resources"]
                .Split(',')
                .ToList()
                .ForEach(s =>
                {
                    Resource r = new ResourceFactory().GetResource(s);
                    if (r != null)
                        resources.Add(new ResourceFactory().GetResource(s));
                    
                }); 

            StateMonitor monitor = new StateMonitor(20000, resources);
            Task.Factory.StartNew(async() => await monitor.Init());

            
        }

       
    }
}
