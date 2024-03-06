using Abstractions;
using EppoiBackend.Dtos;
using Grains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Orleans.Runtime;
using OrleansDashboard.Model;
using System;
using System.Text.Json;

namespace EppoiBackend.Controllers
{
    //[ApiController]
    //[Route("api/itinerary/")]
    public class ItineraryController/* : ControllerBase*/
    {
        //private readonly IGrainFactory _grainFactory;
        //private readonly Random _random = new();
        //private readonly IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> _converter;

        //public ItineraryController(IGrainFactory grainFactory, IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> converter)
        //{
        //    _grainFactory = grainFactory;
        //    _converter = converter;
        //}

        //[HttpGet("{id}")]
        //public async ValueTask<IActionResult> GetItineraryState(string id)
        //{
        //    IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{id}");
        //    //
        //    var state = await itineraryGrain.GetState();
        //    if (state.Id != 0)
        //    {
        //        ItineraryStateDto toReturn = await _converter.ConvertToDto(itineraryGrain);
        //        return Ok(toReturn);
        //    }
        //    else return NotFound($"Itinerary with this Id: {id} doesn't exist");
        //}

        //[HttpPut("{id}")]
        //public async ValueTask<IActionResult> UpdateItinerary(string id, [FromBody] ItineraryState state)
        //{
        //    IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{id}");
        //    ItineraryState itState = await itineraryGrain.GetState();
        //    if (itState.Id != 0)
        //    {
        //        await itineraryGrain.SetState(null, state.Name, state.Description, state.Pois);
        //        ItineraryStateDto toReturn = await _converter.ConvertToDto(itineraryGrain);
        //        return Ok(toReturn);
        //    } else return NotFound($"Itinerary with this Id: {id} doesn't exist");
        //}
        
        ////TODO: Handle the case when the random long generator produces a value that already exists.
        //[HttpPost]
        //public async ValueTask<IActionResult> CreateItinerary([FromBody] ItineraryState state)
        //{
        //    long grainId = _random.NextInt64();
        //    IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{grainId}");
        //    ItineraryState itState = await itineraryGrain.GetState();
        //    Console.WriteLine($"initial state is :{JsonSerializer.Serialize(itState)}");
        //    if (itState.Id == 0)
        //    {
        //        await itineraryGrain.SetState(grainId, state.Name, state.Description, state.Pois);
        //        var toReturn = await _converter.ConvertToDto(itineraryGrain);
        //        return Ok(toReturn);
        //    }
        //    else return BadRequest($"Itinerary with this Id: {grainId} already exists");
        //}
    }
}
