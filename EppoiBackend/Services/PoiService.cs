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

        /// <summary>
        /// Create a new poi based on the PoiState passed
        /// </summary>
        /// <param name="state">the state of the new POI</param>
        /// <returns>A Task that represent the PoiState of the created POI</returns>
        /// <exception cref="Exception">throws an timeoutException after he could not generate a new id for 10 times</exception>
        public async Task<PoiState> CreatePoi(PoiState state)
        {
            long idToSet = rnd.NextInt64();
            int tries = 0;
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            while (await poiCollectionGrain.PoiExists(idToSet))
            {
                if (tries >= 10) throw new Exception("Lot of tries to find a valid id for a new itinerary");
                idToSet = rnd.NextInt64();
                tries++;
            }
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{idToSet}");
            await poiGrain.SetState(idToSet, state.Name, state.Description, state.Address, state.TimeToVisit, state.Coords);
            await poiCollectionGrain.AddPoi(idToSet);
            return await poiGrain.GetPoiState();
        }

        /// <summary>
        /// Retrieve a specific POI
        /// </summary>
        /// <param name="id">the id of the POI</param>
        /// <returns>A Task that represent the PoiState of the POI retrieved</returns>
        public async Task<PoiState> GetAPoi(long id)
        {
            await ThrowExceptionIfPoiNotExists(_grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID), id);
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            return await poiGrain.GetPoiState();
        }

        /// <summary>
        /// Retrieve all POIs
        /// </summary>
        /// <returns>A Task that represent</returns>
        public async Task<List<PoiState>> GetAllPois()
        {
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            return await poiCollectionGrain.GetAllPois();
        }

        /// <summary>
        /// Convert a PoiState to its DTO counterpart
        /// </summary>
        /// <param name="poiState">the PoiState to convert</param>
        /// <returns>The converted PoiState as DTO</returns>
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

        /// <summary>
        /// Convert a list of PoiState to their DTO counterparts
        /// </summary>
        /// <param name="poiStates">the list of PoiState to convert</param>
        /// <returns>The converted list of PoiState as a list of DTOs </returns>
        public List<PoiStateDto> ConvertToDto(List<PoiState> poiStates)
        {
            List<PoiStateDto> toReturn = new ();
            poiStates.ForEach(poi =>
            {
                toReturn.Add(ConvertToDto(poi));
            });
            return toReturn;
        }

        /// <summary>
        /// Retrieve all POIs with the specified ids
        /// </summary>
        /// <param name="poiIDs">the list of ids of POIs to retrieve</param>
        /// <returns>A Task that represent the list of POIs retrieved</returns>
        public async Task<List<PoiState>> GetPois(List<long> poiIDs)
        {
            List<PoiState> dtos = await GetAllPois();
            var toReturn = dtos.Where(dto => poiIDs.Contains(dto.Id)).ToList();
            return toReturn;
        }

        /// <summary>
        /// Update a specific POI based on the PoiState passed
        /// </summary>
        /// <param name="poiID">the id of the POI to update</param>
        /// <param name="state">the new state for the POI</param>
        /// <returns>A Task that represent the updated PoiState</returns>
        public async Task<PoiState> UpdatePoi(long poiID, PoiState state)
        {
            await ThrowExceptionIfPoiNotExists(_grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID), poiID);
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{poiID}");
            await poiGrain.SetState(poiID, state.Name, state.Description, state.Address, state.TimeToVisit, state.Coords);
            return await poiGrain.GetPoiState();
        }

        /// <summary>
        /// Delete a specific POI
        /// </summary>
        /// <param name="id">the id of the POI to delete</param>
        /// <returns>A Task</returns>
        public async Task DeletePoi(long id)
        {
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            await ThrowExceptionIfPoiNotExists(poiCollectionGrain,id);
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            await poiGrain.ClearState();
            await poiCollectionGrain.RemovePoi(id);
            await itineraryCollectionGrain.RemovePoiFromItineraries(id);
        }

        private async Task ThrowExceptionIfPoiNotExists(IPoiCollectionGrain collectionGrain, long idToCheck)
        {
            if (!await collectionGrain.PoiExists(idToCheck)) throw new ArgumentException($"Poi with id:{idToCheck} doesn't exist");
        }
    }
}
