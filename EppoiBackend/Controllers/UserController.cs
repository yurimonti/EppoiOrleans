using Abstractions;
using EppoiBackend.Dtos;
using EppoiBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System;

namespace EppoiBackend.Controllers
{
    [ApiController]
    [Route("api/user/")]
    //TODO: add here the responsibility to call the conversion method for state to dtos, insted of deletegate it to the PoiService
    public class UserController : ControllerBase
    {
        private readonly IPoiService _poiService;
        private readonly IStateToDtoConverter<IPoiGrain, PoiStateDto> _converter;

        public UserController(IPoiService poiService, IStateToDtoConverter<IPoiGrain, PoiStateDto> converter)
        {
            _poiService = poiService;
            _converter = converter;
        }

        [HttpGet("poi")]
        public async ValueTask<IActionResult> GetPois()
        {
            List<PoiStateDto> pois = await _poiService.GetAllPois();
            return Ok(pois);
        }

        [HttpGet("poi/{id}")]
        public async ValueTask<IActionResult> GetPoiState(string id)
        {
            PoiStateDto toReturn = await _poiService.GetAPoi(long.Parse(id));
            return Ok(toReturn);
        }

        [HttpPost("poi/")]
        public async ValueTask<IActionResult> CreatePoi([FromBody] PoiStateDto state)
        {
            PoiState stateToReturn = await _poiService.CreatePoi(state);
            return Ok(stateToReturn);
        }
    }
}
