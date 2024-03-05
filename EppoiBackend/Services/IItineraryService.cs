using Abstractions;
using EppoiBackend.Dtos;
using OpenTelemetry.Resources;

namespace EppoiBackend.Services
{
    public interface IItineraryService
    {
        Task<ItineraryState> CreateItinerary(ItineraryStateDto state);
        Task<List<ItineraryStateDto>> GetAllItineraries(Func<ItineraryStateDto, bool>? predicate);
        Task<ItineraryStateDto> GetAnItinerary(long itineraryID);
        Task<ItineraryStateDto> UpdateItinerary(long itineraryID, ItineraryStateDto state);
    }
}
