using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Net;
using System.Text.Json;
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
        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"EnteGrain: {this.GetGrainId} was just activated");
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"EnteGrain: {this.GetGrainId} was just deactivated");
            return base.OnDeactivateAsync(reason, cancellationToken);
        }


        public async ValueTask<EnteState> GetState()
        {
            _logger.LogInformation($"EnteGrain: {this.GetGrainId} retrieved its state");
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(_state.State)}");
            return await Task.FromResult(_state.State);
        }

        public async ValueTask SetState(Guid id,string username, string city, List<long> poiIDs)
        {
            _state.State.Id = id;
            _state.State.City = city;
            _state.State.PoiIDs = poiIDs;
            _state.State.Username = username;
            await _state.WriteStateAsync();
            _logger.LogInformation($"EnteGrain: {this.GetGrainId} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }

        public async ValueTask SetState(string username, string city, List<long> poiIDs)
        {
            _state.State.City = city;
            _state.State.PoiIDs = poiIDs;
            _state.State.Username = username;
            await _state.WriteStateAsync();
            _logger.LogInformation($"EnteGrain: {this.GetGrainId} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }
    }
}
