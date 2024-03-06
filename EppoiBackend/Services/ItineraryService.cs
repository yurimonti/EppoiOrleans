using Abstractions;
using EppoiBackend.Dtos;

namespace EppoiBackend.Services
{
    //TODO: use State instead of DTOs
    public class ItineraryService : IItineraryService
    {
        private readonly Random rnd = new();
        private readonly IGrainFactory _grainFactory;
        private static readonly string ITINERARY_COLLECTION_ID = "itinerary-collection";
        private readonly IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> _converter;
        private readonly IPoiService _poiService;

        public ItineraryService(IGrainFactory grainFactory, IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> converter, IPoiService poiService)
        {
            _grainFactory = grainFactory;
            _converter = converter;
            _poiService = poiService;
        }
        
        public async Task<ItineraryState> CreateItinerary(ItineraryStateDto state)
        {
            long idToSet = rnd.NextInt64();
            int tries = 0;
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            while (await itineraryCollectionGrain.ItineraryExists(idToSet))
            {
                if (tries >= 10) throw new Exception("Lot of tries to find a valid id for a new itinerary");
                idToSet = rnd.NextInt64();
                tries++;
            }
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{idToSet}");
            await itineraryGrain.SetState(idToSet, state.Name, state.Description, state.Pois.Select(p => p.Id).ToList());
            await itineraryCollectionGrain.AddItinerary(idToSet);
            return await itineraryGrain.GetState();
        }

        public async Task<List<ItineraryStateDto>> GetAllItineraries(Func<ItineraryStateDto, bool>? predicate)
        {
            Console.WriteLine($"itinerary collection grain should be activated in a while");
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            Console.WriteLine($"itinerary collection grain has been activated");
            List<ItineraryState> itineraries = await itineraryCollectionGrain.GetAllItineraries();
            IEnumerable<ItineraryStateDto> toReturn = itineraries.Select(async ev => await ConvertToDto(ev))
                   .Select(t => t.Result)
                   .Where(i => i != null);
            if(predicate != null)
                toReturn = toReturn.Where(predicate);
            return toReturn.ToList();
        }
        private async Task<ItineraryStateDto> ConvertToDto(ItineraryState itineraryState)
        {
            List<PoiStateDto> poisToAdd = await _poiService.GetPois(itineraryState.Pois);
            return new ItineraryStateDto
            {
                Name = itineraryState.Name,
                Description = itineraryState.Description,
                Pois = poisToAdd,
                Id = itineraryState.Id,
                TimeToVisit = (double)itineraryState.TimeToVisit
            };
        }

        public async Task<ItineraryStateDto> GetAnItinerary(long itineraryID)
        {
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await ThrowExceptionIfItineraryNotExists(itineraryCollectionGrain, itineraryID);
            IItineraryGrain itGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{itineraryID}");
            var itState = await itGrain.GetState();
            return await ConvertToDto(itState);
        }

        public async Task<ItineraryStateDto> UpdateItinerary(long itineraryID, ItineraryStateDto state)
        {
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await ThrowExceptionIfItineraryNotExists(itineraryCollectionGrain, itineraryID);
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{itineraryID}");
            await itineraryGrain.SetState(itineraryID, state.Name, state.Description, state.Pois.Select(p => p.Id).ToList());
            return await ConvertToDto(await itineraryGrain.GetState());
        }

        public async Task DeleteItinerary(long id)
        {
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await ThrowExceptionIfItineraryNotExists(itineraryCollectionGrain, id);
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{id}");
            await itineraryGrain.ClearState();
            await itineraryCollectionGrain.RemoveItinerary(id);
        }

        private async Task ThrowExceptionIfItineraryNotExists(IItineraryCollectionGrain collectionGrain,long idToCheck)
        {
            if (!await collectionGrain.ItineraryExists(idToCheck)) throw new ArgumentException($"Itinerary with id:{idToCheck} doesn't exist");
        }
    }
}
