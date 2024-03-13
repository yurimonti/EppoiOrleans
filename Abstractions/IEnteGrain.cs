namespace Abstractions
{
    public interface IEnteGrain: IGrainWithGuidKey
    {
        ValueTask<EnteState> GetState();
        //ValueTask SetState(Guid id, string username,string city,List<long> poiIDs);
        ValueTask SetState(string username,string city,List<long> poiIDs);
        Task<PoiState> CreatePoi(PoiState toCreate);
        Task<PoiState> UpdatePoi(PoiState newState,long poiToUpdate);
        Task RemovePoi(long toRemove);
        Task<List<PoiState>> GetPois(bool mineOnly);
        Task<PoiState> GetPoiState(long toRetrieve);
    }
}
