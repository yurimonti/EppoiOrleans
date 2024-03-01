namespace Abstractions
{
    public interface IItineraryGrain : IGrainWithStringKey
    {
        ValueTask<ItineraryState> GetState();
        ValueTask SetState(long id, string name, string description, List<long> pois);
        ValueTask<List<PoiState>> GetPois();
    }
}
