using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IEnteGrain: IGrainWithStringKey
    {
        ValueTask<EnteState> GetState();
        ValueTask SetState(Guid id, string username,string city,List<long> poiIDs);
        ValueTask SetState(string username,string city,List<long> poiIDs);
    }
}
