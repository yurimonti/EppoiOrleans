namespace Abstractions
{
    public interface IPoiGrain : IGrainWithStringKey
    {

        Task<PoiState> GetState();
        Task<PoiState> SetState(string name, string description, string address, double timeToVisit, Coordinate coordinate);
        Task ClearState();
    }
}
