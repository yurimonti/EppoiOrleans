using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IPoiCollectionGrain : IGrainWithStringKey
    {
        ValueTask<List<long>> GetAllPoisIds();
        ValueTask AddPoi(long id);
        ValueTask RemovePoi(long id);
        Task<List<PoiState>> GetAllPois();
    }
}
