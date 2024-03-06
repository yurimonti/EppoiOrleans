using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Text.Json;

namespace Grains
{
    public class PoiGrain : Grain, IPoiGrain
    {
        private readonly ILogger<IPoiGrain> _logger;
        private readonly IPersistentState<PoiState> _state;

        public PoiGrain(ILogger<IPoiGrain> logger,
            [PersistentState("Poi")]
            IPersistentState<PoiState> state)
        {
            _logger = logger;
            _state = state;
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"PoiGrain: {this.GetGrainId} was just activated");
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"PoiGrain: {this.GetGrainId} was just deactivated");
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task<PoiState> GetPoiState()
        {
            PoiState state = _state.State;
            _logger.LogInformation($"PoiGrain: {this.GetGrainId} retrieved its state");
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public async Task SetState(long id,string name, string description, string address, double timeToVisit, Coordinate coordinate)
        {
            _state.State.Name = name;
            _state.State.Address = address;
            _state.State.Id = id;
            _state.State.Description = description;
            _state.State.TimeToVisit = timeToVisit;
            _state.State.Coords = coordinate;
            await _state.WriteStateAsync();
            _logger.LogInformation($"PoiGrain: {this.GetGrainId} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }

        public async Task ClearState()
        {
            await _state.ClearStateAsync();
            _logger.LogInformation($"PoiGrain: {this.GetGrainId} state was just cleared");
        }
    }
}
