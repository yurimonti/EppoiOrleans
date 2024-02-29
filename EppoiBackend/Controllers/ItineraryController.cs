using Abstractions;
using EppoiBackend.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EppoiBackend.Controllers
{
    [ApiController]
    [Route("api/itinerary/")]
    public class ItineraryController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;
        private readonly Random _random = new Random();

        public ItineraryController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet("{id}")]
        public async ValueTask<ItineraryState> GetItineraryState(string id)
        {
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{id}");
            return await itineraryGrain.GetState();
        }

        [HttpPut("{id}")]
        public async ValueTask<IActionResult> UpdateItinerary(string id, PoiStateDto state)
        {
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            await poiGrain.SetState(long.Parse(id), state.Name, state.Description, state.Address, state.TimeToVisit, state.Coordinate);
            return Ok();
        }
        //TODO: vedere perche non funziona il for
        [HttpPost]
        public async ValueTask<IActionResult> CreateItinerary([FromBody] ItineraryStateDto state)
        {
            long grainId = _random.NextInt64();
            double timeToVisit = CalcTime(state.Pois);
            Console.WriteLine($"final timeToVisit : {timeToVisit}");
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{grainId}");
            await itineraryGrain.SetState(grainId, state.Name, state.Description, timeToVisit, state.Pois);
            return Ok();
        }

        private double CalcTime(List<long> pois)
        {
            double timeToVisit = 0;
            pois.ForEach(async poiId =>
            {
                var poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{poiId}");
                var state = await poiGrain.GetPoiState();
                Console.WriteLine(JsonSerializer.Serialize(state));
                timeToVisit += state.TimeToVisit;
                Console.WriteLine($"computed timeToVisit : {timeToVisit}");
            });
            return timeToVisit;
        }
    }
}
