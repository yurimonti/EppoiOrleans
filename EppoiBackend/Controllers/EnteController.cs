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
        private readonly IPoiService _poiService;
        private readonly IStateToDtoConverter<IPoiGrain, PoiStateDto> _converter;
        private readonly IGrainFactory _grainFactory;

        public record EnteInfo
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }
            [JsonPropertyName("city")]
            public string City { get; set; }
        }

        public EnteController(IPoiService poiService, IStateToDtoConverter<IPoiGrain, PoiStateDto> converter, IGrainFactory grainFactory)
        {
            _poiService = poiService;
            _converter = converter;
            _grainFactory = grainFactory;
        }

        private bool IsAnEnte(EnteState enteState, Guid id)
        {
            return enteState.Id == id && enteState.Role == UserRole.Ente;
        }

        [HttpPost("signup")]
        public async ValueTask<IActionResult> SignUp([FromBody] EnteInfo enteInfo)
        {
            Guid id = Guid.NewGuid();
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente{id}");
            await enteGrain.SetState(id, enteInfo.Username, enteInfo.City, new List<long>());
            EnteState state = await enteGrain.GetState();
            return Ok(state);
        }

        [HttpGet("{id}/poi")]
        public async ValueTask<IActionResult> GetPois(Guid id)
        {
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente{id}");
            EnteState state = await enteGrain.GetState();
            if (IsAnEnte(state, id))
            {
                List<long> entePois = state.PoiIDs;
                var toReturn = await _poiService.GetPois(entePois);
                return Ok(toReturn);
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/poi/{poiId}")]
        public async ValueTask<IActionResult> GetPoiState(Guid id, string poiId)
        {
            //TODO:returns ente pois instead the whole state of an ente
            //PoiStateDto stateDtos = await _poiService.GetAPoi(long.Parse(poiId));
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    var poi = await _poiService.GetAPoi(long.Parse(poiId));
                    return Ok(poi);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            else return Unauthorized();
        }

        [HttpPost("{id}/poi/")]
        public async ValueTask<IActionResult> CreatePoi([FromBody] PoiStateDto state, Guid id)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    PoiState stateToReturn = await _poiService.CreatePoi(state);
                    enteState.PoiIDs.Add(stateToReturn.Id);
                    await enteGrain.SetState(enteState.City, enteState.Username, enteState.PoiIDs);
                    return Ok(stateToReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else return Unauthorized();
        }


        [HttpPut("{id}/poi/{poiId}")]
        public async ValueTask<IActionResult> UpdatePoi([FromBody] PoiStateDto state, Guid id, string poiId)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    PoiStateDto stateToReturn = await _poiService.UpdatePoi(long.Parse(poiId), state);
                    return Ok(stateToReturn);
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
            IEnteGrain enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente{id}");
            EnteState enteState = await enteGrain.GetState();
            if (IsAnEnte(enteState, id))
            {
                try
                {
                    await _poiService.DeletePoi(long.Parse(poiId));
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
