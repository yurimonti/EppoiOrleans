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
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async ValueTask<PoiState> GetPoiState()
        {
            PoiState state = _state.State;
            _logger.LogInformation($"The resulting state for {this.IdentityString} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public async ValueTask SetState(long id,string name, string description, string address, double timeToVisit, (double lat, double lon) coordinate)
        {
            _state.State.Name = name;
            _state.State.Address = address;
            _state.State.Id = id;
            _state.State.Description = description;
            _state.State.TimeToVisit = timeToVisit;
            _state.State.Coordinate = coordinate;
            await _state.WriteStateAsync();
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.IdentityString}");
        }
    }
}
