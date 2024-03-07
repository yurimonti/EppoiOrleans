using Abstractions;
using EppoiBackend.Dtos;
using OpenTelemetry.Resources;

namespace EppoiBackend.Services
{
    public interface IItineraryService
    {
        Task<ItineraryState> CreateItinerary(ItineraryState state);
        Task DeleteItinerary(long id);
        Task<List<ItineraryState>> GetAllItineraries(Func<ItineraryState, bool>? predicate);
        Task<ItineraryState> GetAnItinerary(long itineraryID);
        Task<ItineraryState> UpdateItinerary(long itineraryID, ItineraryState state);
        Task<ItineraryStateDto> ConvertToDto(ItineraryState itineraryState);
        Task<List<ItineraryStateDto>> ConvertToDto(List<ItineraryState> itineraryStates);
    }
}
