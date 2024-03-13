namespace Abstractions
{
    public interface IUserGrain : IGrainWithGuidKey
    {
        Task<UserState> GetState();
        Task SetState(string username, List<long> itineraryIDs);
        Task<List<ItineraryState>> GetItineraries(bool mineOnly);
        Task<ItineraryState> GetItineraryState(long toRetrieve);
        Task<List<PoiState>> GetPois();
        Task<PoiState> GetPoiState(long toRetrieve);
        Task RemoveItinerary(long toRemove);
        Task<ItineraryState> UpdateItinerary(ItineraryState newState, long itineraryToUpdate);
        Task<ItineraryState> CreateItinerary(ItineraryState toCreate);
    }
}
