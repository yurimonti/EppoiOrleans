using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IItineraryGrain : IGrainWithStringKey
    {
        ValueTask<ItineraryState> GetState();
        ValueTask SetState(long id, string name, string description, double timeToVisit, List<long> pois);
    }
}
