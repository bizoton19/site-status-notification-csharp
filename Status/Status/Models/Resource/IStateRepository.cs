using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Status;
namespace Status
{
    public interface IStateRepository
    {
        Task Save(ICollection<State> state);
        Task<IEnumerable<State>> GetStates(string partitionKey="");
        
    }
}
