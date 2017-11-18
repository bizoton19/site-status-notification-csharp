using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public class ResourceFactory
    {
        private const string _resLocationIndicator = "@";
        private const string _resTypeIncidcator = ":";
        public Resource GetResource(string resource)
        {
            if (resource == null)
            {
                return null;
            }
            var serverNameFromType = typeof(Server).Name.ToLowerInvariant();
            var serverName = resource.Contains(":") ? resource.ToLowerInvariant().Split(':')[0] : null;

            if (resource.ToLowerInvariant().Contains("http") && !resource.ToLowerInvariant().Contains("API_KEY") )
            {

                return new HttpResource(new Uri(resource.Trim()));
            }
           

            if (resource.ToLowerInvariant().Contains("server"))
            {
                var uri = serverName.Trim().Split(':')[0];
                return new Server(uri, uri) { ServerName = uri, Url = uri };
            }
           
            if (resource.ToLowerInvariant().Contains("windowsservice")  )
            {
                var fullyQualifiedResource = resource.ToLowerInvariant().Split(':')[0].Trim();
                var server = fullyQualifiedResource.Split('@')[0];
                var uri = fullyQualifiedResource.Split('@')[1];
                return new WindowsService(uri, new Server(server));
            }
            if (resource.ToLowerInvariant().Contains("iisapppool"))
            {
                var fullyQualifiedResource = resource.ToLowerInvariant().Split(':')[0].Trim();
                var server = fullyQualifiedResource.Trim().Split('@')[1];
                var appPoolName = fullyQualifiedResource.Trim().Split('@')[0];
                return new IisAppPool(appPoolName, new Server(server),90);//need to put cpu treshold in config file?
               
            }
            
            return new NullResource();
        }
        
    }
}
