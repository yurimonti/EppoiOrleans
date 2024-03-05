using Abstractions;
using EppoiBackend.Dtos;
using Microsoft.Extensions.Logging;

namespace EppoiBackend.Services
{
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

        //TODO: implement this method
        public Task<ItineraryState> CreateItinerary(ItineraryStateDto state)
        {
            throw new NotImplementedException();
        }

        //TODO: vedere problema
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

        public Task<ItineraryStateDto> GetAnItinerary(long itineraryID)
        {
            throw new NotImplementedException();
        }

        public Task<ItineraryStateDto> UpdateItinerary(long itineraryID, ItineraryStateDto state)
        {
            throw new NotImplementedException();
        }
    }
}
