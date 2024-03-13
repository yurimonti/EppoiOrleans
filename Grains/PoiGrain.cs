using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Text.Json;

namespace Grains
{
    public class PoiGrain : Grain, IPoiGrain
    {
        private readonly ILogger<IPoiGrain> _logger;
        private static readonly string POI_COLLECTION_ID = "poi-collection";
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
            _logger.LogInformation($"PoiGrain: {this.GetPrimaryKeyLong()} was just activated");
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"PoiGrain: {this.GetPrimaryKeyLong()} was just deactivated");
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task<PoiState> GetState()
        {
            PoiState state = _state.State;
            _logger.LogInformation($"PoiGrain: {this.GetGrainId} retrieved its state");
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyLong()} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public async Task<PoiState> SetState(string name, string description, string address, double timeToVisit, Coordinate coordinate)
        {
            PoiState toSet = new() { Id = this.GetPrimaryKeyLong(), Name = name, Description = description, Address = address, TimeToVisit = timeToVisit, Coords = coordinate };
            _state.State = toSet;
            await _state.WriteStateAsync();
            _logger.LogInformation($"PoiGrain: {this.GetPrimaryKeyLong()} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyLong()}");
            return toSet;
        }

        public async Task ClearState()
        {
            await _state.ClearStateAsync();
            _logger.LogInformation($"PoiGrain: {this.GetPrimaryKeyLong()} state was just cleared");
        }

        //private long GetPoiIdFromGrainStringKey()
        //{
        //    return long.Parse(this.GetPrimaryKeyString().Split("/")[1]);
        //}
    }
}
