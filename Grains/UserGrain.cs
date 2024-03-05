using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Text.Json;

namespace Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly IPersistentState<UserState> _state;
        private readonly ILogger<IPoiGrain> _logger;

        public UserGrain([PersistentState("User")] IPersistentState<UserState> state, ILogger<IPoiGrain> logger)
        {
            _state = state;
            _logger = logger;
        }

        public async Task<UserState> GetState()
        {
            UserState state = _state.State;
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public async Task SetState(Guid id, string username, List<long> itineraryIDs)
        {
            _state.State.Username = username;
            _state.State.ItineraryIDs = itineraryIDs;
            _state.State.Id = id;
            await _state.WriteStateAsync();
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }

        public async Task SetState(string username, List<long> itineraryIDs)
        {
            _state.State.Username = username;
            _state.State.ItineraryIDs = itineraryIDs;
            await _state.WriteStateAsync();
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }
    }
}
