namespace Abstractions
{
    public interface IPoiGrain : IGrainWithStringKey
    {

        ValueTask<PoiState> GetPoiState();
        ValueTask SetState(long id, string name, string description,string address,double timeToVisit,(double lat,double lon) coordinate);
    }
}
