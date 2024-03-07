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
        private List<PoiState> _pois;

        public ItineraryGrain(ILogger<IItineraryGrain> logger,
            [PersistentState("Itinerary")]
            IPersistentState<ItineraryState> state)
        {
            _logger = logger;
            _state = state;
            _pois = new();
        }

        public async Task<ItineraryState> GetState()
        {
            await UpdateItineraryParams();
            ItineraryState state = _state.State;
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await UpdateItineraryParams();
            _logger.LogInformation($"ItineraryGrain: {this.GetGrainId} was just activated");
            await base.OnActivateAsync(cancellationToken);
        }

        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            await UpdateItineraryParams();
            _logger.LogInformation($"ItineraryGrain: {this.GetGrainId} was just deactivated");
            _pois.Clear();
            await base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task SetState(long? id, string name, string description, List<long> pois)
        {
            _state.State.Name = name;
            _state.State.Pois = pois;
            if(id != null) _state.State.Id = (long)id;
            _state.State.Description = description;
            //_state.State.TimeToVisit = timeToVisit;
            await _state.WriteStateAsync();
            await UpdateItineraryParams();
            _logger.LogInformation($"ItineraryGrain: {this.GetGrainId} sett its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is set to {this.GetPrimaryKeyString()}");
        }

        /**
         * Set
         **/
        private async Task UpdateItineraryParams()
        {
            _pois.Clear();
            if (_state is not { State.Pois.Count: > 0 }) {
                _state.State.TimeToVisit = 0;
                await _state.WriteStateAsync();
                return;
            }
            foreach (var id in  _state.State.Pois)
            {
                _logger.LogTrace("Id of poi ->", id);
                IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>($"poi{id}");
                PoiState poiState = await poiGrain.GetPoiState();
                _pois.Add(poiState);
                _logger.LogInformation($"Poi list information are just loaded to itinerary {this.GetPrimaryKeyString()}");
            };
            _state.State.TimeToVisit = _pois.Select(poi => poi.TimeToVisit).Sum();
            await _state.WriteStateAsync();
            _logger.LogInformation($"Time To Visit is just updated for itinerary {this.GetPrimaryKeyString()}");
        }

        public async Task<List<PoiState>> GetPois()
        {
            _logger.LogInformation($"ItineraryGrain: {this.GetGrainId} retrieved its local not persistent state");
            _logger.LogInformation($"The resulting not persistent state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(_pois)}");
            return await Task.FromResult(_pois);
        }
        public async Task ClearState()
        {
            _pois.Clear();
            await _state.ClearStateAsync();
            _logger.LogInformation($"ItineraryGrain: {this.GetGrainId} states were just cleared");
        }

        public async Task RemovePoi(long id)
        {
            _state.State.Pois.Remove(id);
            await _state.WriteStateAsync();
            await UpdateItineraryParams();
        }
    }
}
