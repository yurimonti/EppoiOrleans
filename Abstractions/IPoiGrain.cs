namespace Abstractions
{
    public interface IPoiGrain : IGrainWithStringKey
    {

        Task<PoiState> GetPoiState();
        Task SetState(long id, string name, string description,string address,double timeToVisit,Coordinate coordinate);
        Task ClearState();
    }
}
