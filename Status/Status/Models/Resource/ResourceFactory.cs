using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public class ResourceFactory:AbstractResourceFactory
    {
        public virtual Resource GetResource(string Uri,string Type,string Name="")
        {
            if (Type == null)
            {
                return null;
            }
            if(Type.ToLowerInvariant().Trim().Equals("http") && !Type.ToLowerInvariant().Contains("API_KEY"))
            {

                return new HttpResource(new Uri(Uri.Trim()));
            }
            if (Type.ToLowerInvariant().Equals("server"))
            {
                return new Server(Name, Uri) { ServerName = Name, Url = Uri };
            }

            if (Type.ToLowerInvariant().Contains("windowsservice"))
            {

                return new WindowsService(Name, new Server(Uri));
            }
            if (Type.ToLowerInvariant().Contains("iisapppool"))
            {
               
                return new IisAppPool(Name, new Server(Uri), 90);//need to put cpu treshold in config file?

            }

            return default(Resource);
        }


    }
}
