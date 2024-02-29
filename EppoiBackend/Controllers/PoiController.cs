using Abstractions;
using EppoiBackend.Dtos;
using Microsoft.AspNetCore.Mvc;
using Orleans.Runtime;

namespace EppoiBackend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class PoiController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;
        private readonly Random _random = new Random();

        public PoiController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet("{id}")]
        public async ValueTask<PoiState> GetPoiState(string id)
        {
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            return await poiGrain.GetPoiState();
        }

        [HttpPut("{id}")]
        public async ValueTask<IActionResult> UpdatePoi(string id, PoiStateDto state)
        {
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            await poiGrain.SetState(long.Parse(id), state.Name, state.Description, state.Address, state.TimeToVisit, state.Coordinate);
            return Ok();
        }

        [HttpPost]
        public async ValueTask<IActionResult> CreatePoi([FromBody] PoiStateDto state)
        {
            long grainId = _random.NextInt64();
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{grainId}");
            await poiGrain.SetState(grainId,state.Name,state.Description,state.Address,state.TimeToVisit, state.Coordinate);
            return Ok();
        }
    }
}
