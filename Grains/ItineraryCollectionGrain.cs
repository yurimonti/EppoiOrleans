using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Text.Json;

namespace Grains
{
    public class ItineraryCollectionGrain : Grain, IItineraryCollectionGrain
    {
        private readonly IPersistentState<List<long>> _state;
        private readonly ILogger<IItineraryCollectionGrain> _logger;
        private readonly List<ItineraryState> _itineraries;

        public ItineraryCollectionGrain(
            [PersistentState("ItineraryCollection")] IPersistentState<List<long>> state,
            ILogger<IItineraryCollectionGrain> logger)
        {
            _state = state;
            _logger = logger;
            _itineraries = new();
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"ItineraryCollectionGrain: {this.GetGrainId} was just activated");
            await UpdateParams();
            await base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"ItineraryCollectionGrain: {this.GetGrainId} was just deactivated");
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task AddItinerary(long id)
        {
            _state.State.Add(id);
            await _state.WriteStateAsync();
            await UpdateParams();
            _logger.LogInformation($"ItineraryCollectionGrain: {this.GetGrainId} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }

        public async Task<List<long>> GetAllItineraryIds()
        {
            _logger.LogInformation($"ItineraryCollectionGrain: {this.GetGrainId} retrieved its state");
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(_state.State)}");
            return await Task.FromResult(_state.State);
        }

        public async Task<List<ItineraryState>> GetAllItineraries()
        {
            var result = await Task.FromResult(_itineraries);
            _logger.LogInformation($"ItineraryCollectionGrain: {this.GetGrainId} retrivied its local not persistent state");
            _logger.LogInformation($"The resulting not persistent state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(_itineraries)}");
            return result;
        }

        public async Task RemoveItinerary(long id)
        {
            _state.State.Remove(id);
            await _state.WriteStateAsync();
            await UpdateParams();
            _logger.LogInformation($"ItineraryCollectionGrain: {this.GetGrainId} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }

        private async Task UpdateParams()
        {
            _itineraries.Clear();
            if (_state is not { State.Count: > 0 }) return;
            foreach (var id in _state.State)
            {
                _logger.LogTrace("Id of itinerary ->", id);
                IItineraryGrain itineraryGrain = GrainFactory.GetGrain<IItineraryGrain>($"itinerary{id}");
                ItineraryState itineraryState = await itineraryGrain.GetState();
                _itineraries.Add(itineraryState);
                _logger.LogInformation($"Itinerary list information are just loaded to collection {this.GetPrimaryKeyString()}");
            };
        }
        //TODO: maybe is useless
        public async Task<bool> ItineraryExists(long id)
        {
            return await Task.FromResult(_state.State.Contains(id));
        }
    }
}
