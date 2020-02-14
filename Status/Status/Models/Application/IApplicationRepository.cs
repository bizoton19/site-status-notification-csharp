using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
    public interface IApplicationRepository
    {
        Task<IEnumerable<Resource>> GetApplications(string partitionKey = "", int currIndex = 0, int pageSize = 25, Resource resource = null);
        Task<bool> ApplicationExist(Status.ApplicationId appId);
        Task CreateOrUpdateApplication(Application app, List<Resource> resources);
    }
}
