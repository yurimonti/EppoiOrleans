using Abstractions;
using EppoiBackend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EppoiBackend.Controllers
{
    //TODO: handle rest controller as the itinerary one ( check if exists, return http status etc...)
    //[ApiController]
    //[Route("api/poi/")]
    public class PoiController /*: ControllerBase*/
    {
        //private readonly IGrainFactory _grainFactory;
        //private readonly Random _random = new();
        //private readonly IStateToDtoConverter<IPoiGrain, PoiStateDto> _converter;

        //public PoiController(IGrainFactory grainFactory, IStateToDtoConverter<IPoiGrain, PoiStateDto> converter)
        //{
        //    _grainFactory = grainFactory;
        //    _converter = converter;
        //}

        //[HttpGet("{id}")]
        //public async ValueTask<IActionResult> GetPoiState(string id)
        //{
        //    IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
        //    PoiStateDto toReturn = await _converter.ConvertToDto(poiGrain);
        //    return Ok(toReturn);
        //}

        //[HttpPut("{id}")]
        //public async ValueTask<IActionResult> UpdatePoi(string id, PoiStateDto state)
        //{
        //    IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
        //    await poiGrain.SetState(long.Parse(id), state.Name, state.Description, state.Address, state.TimeToVisit, state.Coordinate);
        //    return Ok();
        //}

        //[HttpPost]
        //public async ValueTask<IActionResult> CreatePoi([FromBody] PoiStateDto state)
        //{
        //    long grainId = _random.NextInt64();
        //    IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{grainId}");
        //    await poiGrain.SetState(grainId,state.Name,state.Description,state.Address,state.TimeToVisit, state.Coordinate);
        //    return Ok();
        //}
    }
}
