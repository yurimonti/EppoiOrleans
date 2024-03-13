using Abstractions;
using EppoiBackend.Dtos;

namespace EppoiBackend.Services
{
    public interface IItineraryService
    {
        Task<ItineraryStateDto> ConvertToDto(ItineraryState itineraryState);
        Task<List<ItineraryStateDto>> ConvertToDto(List<ItineraryState> itineraryStates);
    }
}
