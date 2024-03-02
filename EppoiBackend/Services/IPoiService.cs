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
    }
}
