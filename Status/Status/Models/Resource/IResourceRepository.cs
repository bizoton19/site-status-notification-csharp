using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status
{
   public interface IResourceRepository
    {
        Task Save(ICollection<State> state);
        Task<IEnumerable<Resource>> GetResources(string partitionKey="",Resource clause=null);
    }
}
