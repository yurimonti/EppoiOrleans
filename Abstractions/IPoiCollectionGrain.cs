using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IPoiCollectionGrain : IGrainWithStringKey
    {
        Task<List<long>> GetAllPoisIds();
        Task AddPoi(long id);
        Task RemovePoi(long id);
        Task<List<PoiState>> GetAllPois();
        Task<bool> PoiExists(long id);
    }
}
