namespace Abstractions
{
    public interface IItineraryGrain : IGrainWithStringKey
    {
        Task<ItineraryState> GetState();
        Task SetState(long? id, string name, string description, List<long> pois);
        Task<List<PoiState>> GetPois();
        Task ClearState();
    }
}
