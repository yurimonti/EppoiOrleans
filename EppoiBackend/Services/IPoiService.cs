using Abstractions;
using EppoiBackend.Dtos;

namespace EppoiBackend.Services
{
    public interface IPoiService
    {
        PoiStateDto ConvertToDto(PoiState state);
        List<PoiStateDto> ConvertToDto(List<PoiState> states);

    }
}
