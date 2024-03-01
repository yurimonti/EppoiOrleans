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
            _pois = new List<PoiState>();
        }

        public async ValueTask<ItineraryState> GetState()
        {
            ItineraryState state = _state.State;
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(state)}");
            await UpdateItineraryParams();
            return await ValueTask.FromResult(state);
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //await UpdateItineraryParams();
            //_logger.LogInformation($"itinerary grain with id: {this.GetPrimaryKeyString()} is just activated");
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            //await UpdateItineraryParams();
            //_logger.LogInformation($"itinerary grain with id: {this.GetPrimaryKeyString()} is just deactivated");
            _pois.Clear();
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async ValueTask SetState(long id, string name, string description, List<long> pois)
        {
            _state.State.Name = name;
            _state.State.Pois = pois;
            _state.State.Id = id;
            _state.State.Description = description;
            //_state.State.TimeToVisit = timeToVisit;
            await _state.WriteStateAsync();
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
            await UpdateItineraryParams();
        }

        //Todo: source cannot be null, this error happens when grain is activating and no state is present yet
        private async ValueTask UpdateItineraryParams()
        {
            _pois.Clear();
            //if (_state is not { State.Pois.Count: > 0 }) return;
            foreach (var id in  _state.State.Pois)
            {
                _logger.LogTrace("Id of poi ->", id);
                IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>($"poi{id}");
                PoiState poiState = await poiGrain.GetPoiState();
                _pois.Add(poiState);
                _logger.LogInformation($"Poi list information are just loaded to itinerary {this.GetPrimaryKeyString()}");
            };
            _state.State.TimeToVisit = _pois.Select(poi => poi.TimeToVisit).Sum();
            _logger.LogInformation($"Time To Visit is just updated for itinerary {this.GetPrimaryKeyString()}");
        }

        public async ValueTask<List<PoiState>> GetPois()
        {
            return await ValueTask.FromResult(_pois);
        }
    }
}
