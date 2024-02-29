using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Text.Json;

namespace Grains
{
    public class ItineraryGrain : Grain, IItineraryGrain
    {
        private readonly ILogger<IItineraryGrain> _logger;
        private readonly IPersistentState<ItineraryState> _state;

        public ItineraryGrain(ILogger<IItineraryGrain> logger,
            [PersistentState("Itinerary")]
            IPersistentState<ItineraryState> state)
        {
            _logger = logger;
            _state = state;
        }

        public async ValueTask<ItineraryState> GetState()
        {
            ItineraryState state = _state.State;
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async ValueTask SetState(long id, string name, string description, double timeToVisit, List<long> pois)
        {
            _state.State.Name = name;
            _state.State.Pois = pois;
            _state.State.Id = id;
            _state.State.Description = description;
            _state.State.TimeToVisit = timeToVisit;
            await _state.WriteStateAsync();
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }
    }
}
