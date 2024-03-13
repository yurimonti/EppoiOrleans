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
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyLong()} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await UpdateItineraryParams();
            _logger.LogInformation($"ItineraryGrain: {this.GetPrimaryKeyLong()} was just activated");
            await base.OnActivateAsync(cancellationToken);
        }

        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            await UpdateItineraryParams();
            _logger.LogInformation($"ItineraryGrain: {this.GetPrimaryKeyLong()} was just deactivated");
            _pois.Clear();
            await base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task<ItineraryState> SetState(string name, string description, List<long> pois)
        {
            _state.State = new() { Id = this.GetPrimaryKeyLong(), Name = name, Description = description, Pois = pois };
            await _state.WriteStateAsync();
            await UpdateItineraryParams();
            _logger.LogInformation($"ItineraryGrain: {this.GetPrimaryKeyLong()} sett its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is set to {this.GetPrimaryKeyLong()}");
            return _state.State;
        }

        public async Task<List<PoiState>> GetPois()
        {
            _logger.LogInformation($"ItineraryGrain: {this.GetPrimaryKeyLong()} retrieved its local not persistent state");
            _logger.LogInformation($"The resulting not persistent state for {this.GetPrimaryKeyLong()} is {JsonSerializer.Serialize(_pois)}");
            return await Task.FromResult(_pois);
        }

        public async Task ClearState()
        {
            _pois.Clear();
            await _state.ClearStateAsync();
            _logger.LogInformation($"ItineraryGrain: {this.GetPrimaryKeyLong()} states were just cleared");
        }

        /**
         * Set
         **/
        private async Task UpdateItineraryParams()
        {
            _pois.Clear();
            if (_state is not { State.Pois.Count: > 0 })
            {
                _state.State.TimeToVisit = 0;
                await _state.WriteStateAsync();
                return;
            }
            foreach (var id in _state.State.Pois)
            {
                _logger.LogTrace("Id of poi ->", id);
                IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>(id);
                PoiState poiState = await poiGrain.GetState();
                _pois.Add(poiState);
                _logger.LogInformation($"Poi list information are just loaded to itinerary {this.GetPrimaryKeyLong()}");
            };
            _state.State.TimeToVisit = _pois.Select(poi => poi.TimeToVisit).Sum();
            await _state.WriteStateAsync();
            _logger.LogInformation($"Time To Visit is just updated for itinerary {this.GetPrimaryKeyLong()}");
        }

        //private long GetPoiIdFromGrainStringKey()
        //{
        //    return long.Parse(this.GetPrimaryKeyString().Split("/")[1]);
        //}

    }
}
