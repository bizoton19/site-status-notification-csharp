using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
    /// <summary>
    /// need to implement as abstract factory
    /// </summary>
   public class ResourceFactory
    {
        public Resource GetResource(string resource)
        {
            if (resource == null)
            {
                return null;
            }
            string serverNameFromType = typeof(WindowsServer).Name.ToLowerInvariant();
            string serverName = resource.Contains(":") ? resource.ToLowerInvariant().Split(':')[0] : null;

            if (resource.ToLowerInvariant().Contains("http")){

                return new HttpResource(new Uri(resource.Trim()));
            }

            if (resource.ToLowerInvariant().Contains("windowsserver"))
            {
                string uri = serverName.Trim().Split(':')[0];
                return new WindowsServer(uri, uri) { ServerName = uri, Url = uri };
            }
           
            if (resource.ToLowerInvariant().Contains("windowsservice")  )
            {
                string fullyQualifiedResource = resource.ToLowerInvariant().Split(':')[0].Trim();
                string server = fullyQualifiedResource.Split('@')[0];
                string uri = fullyQualifiedResource.Split('@')[1];
                return new WindowsService(uri, new WindowsServer(server));
            }

            return null;
        }
        
    }
}
