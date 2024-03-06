using Abstractions;
using EppoiBackend.Dtos;

namespace EppoiBackend.Services
{
    //TODO: change Dtos with state (also for classes that implement this interface)
    public interface IPoiService
    {
        Task<PoiState> CreatePoi(PoiStateDto state);
        Task<PoiStateDto> GetAPoi(long id);

        Task<List<PoiStateDto>> GetAllPois();

        Task<List<PoiStateDto>> GetPois(List<long> poiIDs);
        Task<PoiStateDto> UpdatePoi(long poiID, PoiStateDto state);
        Task DeletePoi(long id);
    }
}
