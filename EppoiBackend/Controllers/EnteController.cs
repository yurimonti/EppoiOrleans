using Abstractions;
using EppoiBackend.Dtos;
using EppoiBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Orleans;
using System;
using System.Text.Json.Serialization;

namespace EppoiBackend.Controllers
{
    [ApiController]
    [Route("api/ente/")]
    //TODO: add here the responsibility to call the conversion method for state to dtos, insted of deletegate it to the PoiService
    //TODO: ente pois only
    public class EnteController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;
        private readonly IPoiService _poiService;

        public record EnteInfo
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }
            [JsonPropertyName("city")]
            public string City { get; set; }
        }

        public EnteController(IGrainFactory grainFactory, IPoiService poiService)
        {
            _grainFactory = grainFactory;
            _poiService = poiService;
        }

        private bool IsAnEnte(EnteState enteState, Guid id)
        {
            return enteState.Id == id && enteState.Role == UserRole.Ente;
        }

        [HttpPost("signup")]
        public async ValueTask<IActionResult> SignUp([FromBody] EnteInfo enteInfo)
        {
            Guid id = Guid.NewGuid();
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente/{id}");
            await enteGrain.SetState(enteInfo.Username, enteInfo.City, []);
            EnteState state = await enteGrain.GetState();
            return Ok(state);
        }

        [HttpGet("{id}/poi")]
        public async ValueTask<IActionResult> GetPois(Guid id, [FromQuery] bool mineOnly)
        {
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente/{id}");
            EnteState state = await enteGrain.GetState();
            if (IsAnEnte(state, id))
            {
                List<long> entePois = state.PoiIDs;
                var poiStates = await enteGrain.GetPois(mineOnly);
                var toReturn = _poiService.ConvertToDto(poiStates);
                return Ok(toReturn);
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/poi/{poiId}")]
        public async ValueTask<IActionResult> GetPoiState(Guid id, string poiId)
        {
            //TODO:returns ente pois instead the whole state of an ente
            //PoiStateDto stateDtos = await _poiService.GetAPoi(long.Parse(poiId));
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente/{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    var poi = await enteGrain.GetPoiState(long.Parse(poiId));
                    var toReturn = _poiService.ConvertToDto(poi);
                    return Ok(toReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            else return Unauthorized();
        }

        [HttpPost("{id}/poi/")]
        public async ValueTask<IActionResult> CreatePoi([FromBody] PoiState state, Guid id)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente/{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    PoiState stateToReturn = await enteGrain.CreatePoi(state);
                    return Ok(_poiService.ConvertToDto(stateToReturn));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else return Unauthorized();
        }


        [HttpPut("{id}/poi/{poiId}")]
        public async ValueTask<IActionResult> UpdatePoi([FromBody] PoiState state, Guid id, string poiId)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente/{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    PoiState stateToReturn = await enteGrain.UpdatePoi(state,long.Parse(poiId));
                    var toReturn = _poiService.ConvertToDto(stateToReturn);
                    return Ok(toReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else return Unauthorized();
        }

        [HttpDelete("{id}/poi/{poiId}")]
        public async ValueTask<IActionResult> DeletePoi(Guid id, string poiId)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente/{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    await enteGrain.RemovePoi(long.Parse(poiId));
                    return Ok("Poi successfully deleted");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            else return Unauthorized();
        }
    }
}
