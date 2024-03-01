using Abstractions;
using EppoiBackend.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace EppoiBackend.Controllers
{
    [ApiController]
    [Route("api/itinerary/")]
    public class ItineraryController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;
        private readonly Random _random = new Random();
        private readonly IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> _converter;

        public ItineraryController(IGrainFactory grainFactory, IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> converter)
        {
            _grainFactory = grainFactory;
            _converter = converter;
        }

        [HttpGet("{id}")]
        public async ValueTask<ItineraryStateDto> GetItineraryState(string id)
        {
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{id}");
            return await _converter.ConvertToDto(itineraryGrain);
        }

        [HttpPut("{id}")]
        public async ValueTask<IActionResult> UpdateItinerary(string id, PoiStateDto state)
        {
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            await poiGrain.SetState(long.Parse(id), state.Name, state.Description, state.Address, state.TimeToVisit, state.Coordinate);
            return Ok();
        }
        
        [HttpPost]
        public async ValueTask<ItineraryStateDto> CreateItinerary([FromBody] ItineraryState state)
        {
            long grainId = _random.NextInt64();
            //double timeToVisit = 0d;
            //foreach (var poiId in state.Pois)
            //{
            //    var poi = _grainFactory.GetGrain<IPoiGrain>($"poi{poiId}");
            //    PoiState aState = await poi.GetPoiState();
            //    timeToVisit += aState.TimeToVisit;
            //};
            //Console.WriteLine($"final timeToVisit : {timeToVisit}");
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{grainId}");
            await itineraryGrain.SetState(grainId, state.Name, state.Description, state.Pois);
            return await _converter.ConvertToDto(itineraryGrain);
        }
    }
}
