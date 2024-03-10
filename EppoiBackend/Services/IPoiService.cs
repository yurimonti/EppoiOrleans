using Abstractions;
using EppoiBackend.Dtos;

namespace EppoiBackend.Services
{
    //TODO: change Dtos with state (also for classes that implement this interface)
    public interface IPoiService
    {
        PoiStateDto ConvertToDto(PoiState state);
        List<PoiStateDto> ConvertToDto(List<PoiState> states);

    }
}
