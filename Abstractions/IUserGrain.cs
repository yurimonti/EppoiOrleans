using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<UserState> GetState();
        Task SetState(Guid id, string username, List<long> itineraryIDs);
        Task SetState(string username, List<long> itineraryIDs);
    }
}
