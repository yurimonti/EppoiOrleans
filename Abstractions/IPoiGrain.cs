namespace Abstractions
{
    public interface IPoiGrain : IGrainWithIntegerKey
    {
        Task<PoiState> GetState();
        Task<PoiState> SetState(string name, string description, string address, double timeToVisit, Coordinate coordinate);
        Task ClearState();
    }
}
