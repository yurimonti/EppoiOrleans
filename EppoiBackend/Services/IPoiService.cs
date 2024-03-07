using Abstractions;
using EppoiBackend.Dtos;

namespace EppoiBackend.Services
{
    //TODO: change Dtos with state (also for classes that implement this interface)
    public interface IPoiService
    {
        Task<PoiState> CreatePoi(PoiState state);
        Task<PoiState> GetAPoi(long id);

        Task<List<PoiState>> GetAllPois();

        Task<List<PoiState>> GetPois(List<long> poiIDs);
        Task<PoiState> UpdatePoi(long poiID, PoiState state);
        Task DeletePoi(long id);
        PoiStateDto ConvertToDto(PoiState state);
        List<PoiStateDto> ConvertToDto(List<PoiState> states);

    }
}
