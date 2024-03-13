using Abstractions;
using EppoiBackend.Dtos;

namespace EppoiBackend.Services
{
    public class PoiService : IPoiService
    {
        private readonly Random rnd = new();
        private readonly IGrainFactory _grainFactory;
        private static readonly string POI_COLLECTION_ID = "poi-collection";
        private static readonly string ITINERARY_COLLECTION_ID = "itinerary-collection";


        public PoiService(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        //public async Task<PoiState> CreatePoi(PoiState state)
        //{
        //    long idToSet = rnd.NextInt64();
        //    int tries = 0;
        //    IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
        //    while (await poiCollectionGrain.PoiExists(idToSet))
        //    {
        //        if (tries >= 10) throw new Exception("Lot of tries to find a valid id for a new itinerary");
        //        idToSet = rnd.NextInt64();
        //        tries++;
        //    }
        //    IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi/{idToSet}");
        //    await poiGrain.SetState(state.Name, state.Description, state.Address, state.TimeToVisit, state.Coords);
        //    await poiCollectionGrain.AddPoi(idToSet);
        //    return await poiGrain.GetState();
        //}

        //public async Task<PoiState> GetAPoi(long id)
        //{
        //    await ThrowExceptionIfPoiNotExists(_grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID), id);
        //    IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi/{id}");
        //    return await poiGrain.GetState();
        //}

        //public async Task<List<PoiState>> GetAllPois()
        //{
        //    IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
        //    return await poiCollectionGrain.GetAllPois();
        //}

        public PoiStateDto ConvertToDto(PoiState poiState)
        {
            return new PoiStateDto
            {
                Name = poiState.Name,
                Description = poiState.Description,
                Coords = poiState.Coords,
                Address = poiState.Address,
                Id = poiState.Id,
                TimeToVisit = poiState.TimeToVisit
            };
        }
        public List<PoiStateDto> ConvertToDto(List<PoiState> poiStates)
        {
            List<PoiStateDto> toReturn = new();
            poiStates.ForEach(poi =>
            {
                toReturn.Add(ConvertToDto(poi));
            });
            return toReturn;
        }

        //public async Task<List<PoiState>> GetPois(List<long> poiIDs)
        //{
        //    List<PoiState> dtos = await GetAllPois();
        //    var toReturn = dtos.Where(dto => poiIDs.Contains(dto.Id)).ToList();
        //    return toReturn;
        //}

        //public async Task<PoiState> UpdatePoi(long poiID, PoiState state)
        //{
        //    await ThrowExceptionIfPoiNotExists(_grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID), poiID);
        //    IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi/{poiID}");
        //    await poiGrain.SetState(state.Name, state.Description, state.Address, state.TimeToVisit, state.Coords);
        //    return await poiGrain.GetState();
        //}

        //public async Task DeletePoi(long id)
        //{
        //    IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
        //    await ThrowExceptionIfPoiNotExists(poiCollectionGrain,id);
        //    IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
        //    IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi/{id}");
        //    await poiGrain.ClearState();
        //    await poiCollectionGrain.RemovePoi(id);
        //    await itineraryCollectionGrain.RemoveItinerariesWithPoi(id);
        //}

        //private async Task ThrowExceptionIfPoiNotExists(IPoiCollectionGrain collectionGrain, long idToCheck)
        //{
        //    if (!await collectionGrain.PoiExists(idToCheck)) throw new ArgumentException($"Poi with id:{idToCheck} doesn't exist");
        //}
    }
}
