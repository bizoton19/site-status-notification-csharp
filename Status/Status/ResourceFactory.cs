using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public class ResourceFactory
    {
        public Resource GetResource(string resource)
        {
            if (resource == null)
            {
                return null;
            }
            var serverNameFromType = typeof(Server).Name.ToLowerInvariant();
            var serverName = resource.Contains(":") ? resource.ToLowerInvariant().Split(':')[0] : null;

            if (resource.ToLowerInvariant().Contains("http")){

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
            return null;
        }
        public Resource GetResource(Resource resource)
        {

            if (resource== null)
            {
                return null;
            }
            if (resource.GetType() == typeof(Server))
            {
                return new Server(resource.Name, resource.Url);

            }
            if (resource.GetType() == typeof(WindowsService))
            {
                return new WindowsService(resource.Name,resource);

            }
            if (resource.GetType() == typeof(HttpResource))
            {
                return new HttpResource(new Uri(resource.Url)) ;

            }
            
          



            return null;
        }
    }
}
