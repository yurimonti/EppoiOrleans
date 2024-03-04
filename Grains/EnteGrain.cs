using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Net;
using System.Xml.Linq;

namespace Grains
{
    public class EnteGrain : Grain, IEnteGrain
    {
        private readonly ILogger<IEnteGrain> _logger;
        private readonly IPersistentState<EnteState> _state;

        public EnteGrain(ILogger<IEnteGrain> logger, [PersistentState("Ente")] IPersistentState<EnteState> state)
        {
            _logger = logger;
            _state = state;
        }

        public async ValueTask<EnteState> GetState()
        {
            return await Task.FromResult(_state.State);
        }

        public async ValueTask SetState(Guid id,string username, string city, List<long> poiIDs)
        {
            _state.State.Id = id;
            _state.State.City = city;
            _state.State.PoiIDs = poiIDs;
            _state.State.Username = username;
            await _state.WriteStateAsync();
        }

        public async ValueTask SetState(string username, string city, List<long> poiIDs)
        {
            _state.State.City = city;
            _state.State.PoiIDs = poiIDs;
            _state.State.Username = username;
            await _state.WriteStateAsync();
        }
    }
}
