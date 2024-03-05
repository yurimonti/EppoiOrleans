using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IItineraryCollectionGrain : IGrainWithStringKey
    {
        Task<List<ItineraryState>> GetAllItineraries();
        ValueTask AddItinerary(long id);
        ValueTask<List<long>> GetAllItineraryIds();
        ValueTask RemoveItinerary(long id);
    }
}
