using Abstractions;
using EppoiBackend.Dtos;
using System.Text.Json;

namespace EppoiBackend.Services
{
    public class ItineraryService : IItineraryService
    {
        private readonly Random rnd = new();
        private readonly IGrainFactory _grainFactory;
        private static readonly string ITINERARY_COLLECTION_ID = "itinerary-collection";
        private readonly IPoiService _poiService;
        private readonly ILogger<IItineraryService> _logger;

        public ItineraryService(IGrainFactory grainFactory, IPoiService poiService,ILogger<IItineraryService> logger)
        {
            _grainFactory = grainFactory;
            _poiService = poiService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new ItineraryState
        /// </summary>
        /// <param name="state">new ItineraryState to create</param>
        /// <returns>A Task that represent the new ItineraryState</returns>
        /// <exception cref="TimeoutException">throws an timeoutException after he could not generate a new id for 10 times</exception>
        public async Task<ItineraryState> CreateItinerary(ItineraryState state)
        {
            long idToSet = rnd.NextInt64();
            int tries = 0;
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            while (await itineraryCollectionGrain.ItineraryExists(idToSet))
            {
                if (tries >= 10) throw new TimeoutException("Lot of tries to find a valid id for a new itinerary");
                idToSet = rnd.NextInt64();
                tries++;
            }
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{idToSet}");
            await itineraryGrain.SetState(idToSet, state.Name,state.Description,state.Pois);
            await itineraryCollectionGrain.AddItinerary(idToSet);
            return await itineraryGrain.GetState();
        }

        /// <summary>
        /// Retrieve all the itineraries depending on the predicate parameter
        /// </summary>
        /// <param name="predicate">Filter function for itineraries</param>
        /// <returns>A Task that represent the filtered itinerary list</returns>
        public async Task<List<ItineraryState>> GetAllItineraries(Func<ItineraryState, bool>? predicate)
        {
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            List<ItineraryState> itineraries = await itineraryCollectionGrain.GetAllItineraries();
            IEnumerable<ItineraryState> toReturn = itineraries.AsEnumerable();
            if (predicate != null)
                toReturn = itineraries.Where(predicate);
            return toReturn.ToList();
        }

        /// <summary>
        /// Convert an ItineraryState to its DTO counterpart
        /// </summary>
        /// <param name="itineraryState">the ItineraryState to convert</param>
        /// <returns>A Task that represent the converted ItineraryState as DTO </returns>
        public async Task<ItineraryStateDto> ConvertToDto(ItineraryState itineraryState)
        {
            List<PoiState> poiStates = await _poiService.GetPois(itineraryState.Pois);
            List<PoiStateDto> poiDTOs = _poiService.ConvertToDto(poiStates);
            return new ItineraryStateDto
            {
                Name = itineraryState.Name,
                Description = itineraryState.Description,
                Pois = poiDTOs,
                Id = itineraryState.Id,
                TimeToVisit = (double)itineraryState.TimeToVisit
            };
        }

        /// <summary>
        /// Convert a list of ItineraryState to their DTO counterparts
        /// </summary>
        /// <param name="itineraryStates">the list of ItineraryStates to convert</param>
        /// <returns>A Task that represent the converted list of ItineraryState as a list of DTOs </returns>
        public async Task<List<ItineraryStateDto>> ConvertToDto(List<ItineraryState> itineraryStates)
        {
            List<ItineraryStateDto> toReturn = [];
            foreach(var itineraryState in itineraryStates)
            {
                var toAdd = await ConvertToDto(itineraryState);
                toReturn.Add(toAdd);
            };
            return toReturn;
        }

        /// <summary>
        /// Retrieve an itinerary based on its id
        /// </summary>
        /// <param name="itineraryID">the id of the itinerary</param>
        /// <returns>A Task that represent the retrieved itinerary</returns>
        public async Task<ItineraryState> GetAnItinerary(long itineraryID)
        {
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await ThrowExceptionIfItineraryNotExists(itineraryCollectionGrain, itineraryID);
            IItineraryGrain itGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{itineraryID}");
            var state =  await itGrain.GetState();
            return state;
        }

        /// <summary>
        /// Update a specific itinerary based on the ItineraryState passed
        /// </summary>
        /// <param name="itineraryID">the id of the itinerary to update</param>
        /// <param name="state">the new state for the itinerary</param>
        /// <returns>A Task that represent the updated ItineraryState</returns>
        public async Task<ItineraryState> UpdateItinerary(long itineraryID, ItineraryState state)
        {
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await ThrowExceptionIfItineraryNotExists(itineraryCollectionGrain, itineraryID);
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{itineraryID}");
            await itineraryGrain.SetState(itineraryID, state.Name, state.Description, state.Pois);
            return await itineraryGrain.GetState();
        }

        /// <summary>
        /// Delete a specific itinerary
        /// </summary>
        /// <param name="itineraryID">the id of the itinerary to delete</param>
        /// <returns>A Task</returns>
        public async Task DeleteItinerary(long itineraryID)
        {
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await ThrowExceptionIfItineraryNotExists(itineraryCollectionGrain, itineraryID);
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{itineraryID}");
            await itineraryGrain.ClearState();
            await itineraryCollectionGrain.RemoveItinerary(itineraryID);
        }


        private async Task ThrowExceptionIfItineraryNotExists(IItineraryCollectionGrain collectionGrain,long idToCheck)
        {
            if (!await collectionGrain.ItineraryExists(idToCheck)) throw new ArgumentException($"Itinerary with id:{idToCheck} doesn't exist");
        }
    }
}
