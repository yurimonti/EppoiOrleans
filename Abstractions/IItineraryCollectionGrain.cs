namespace Abstractions
{
    public interface IItineraryCollectionGrain : IGrainWithStringKey
    {
        Task<List<ItineraryState>> GetAllItineraries();
        Task<bool> ItineraryExists(long id);
        Task AddItinerary(long id);
        Task<List<long>> GetAllItineraryIds();
        Task RemoveItinerary(long id);

        Task RemovePoiFromItineraries(long removedPoi);
    }
}
