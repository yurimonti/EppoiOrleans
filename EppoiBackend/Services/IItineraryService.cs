using Abstractions;
using EppoiBackend.Dtos;
using OpenTelemetry.Resources;

namespace EppoiBackend.Services
{
    public interface IItineraryService
    {
        Task<ItineraryStateDto> ConvertToDto(ItineraryState itineraryState);
        Task<List<ItineraryStateDto>> ConvertToDto(List<ItineraryState> itineraryStates);
    }
}
