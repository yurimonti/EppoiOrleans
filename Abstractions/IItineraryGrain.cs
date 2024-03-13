namespace Abstractions
{
    public interface IItineraryGrain : IGrainWithIntegerKey
    {
        Task<ItineraryState> GetState();
        Task<ItineraryState> SetState(string name, string description, List<long> pois);
        Task<List<PoiState>> GetPois();
        Task ClearState();
    }
}
