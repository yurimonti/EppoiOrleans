using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public class PoiCollectionGrain : Grain, IPoiCollectionGrain
    {
        private readonly IPersistentState<List<long>> _state;
        private readonly ILogger<IPoiCollectionGrain> _logger;
        private readonly List<PoiState> _pois;

        public PoiCollectionGrain(
            [PersistentState("PoiCollection")] IPersistentState<List<long>> state,
            ILogger<IPoiCollectionGrain> logger)
        {
            _state = state;
            _logger = logger;
            _pois = new();
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _ = UpdateParams();
            return base.OnActivateAsync(cancellationToken);
        }

        public async ValueTask AddPoi(long id)
        {
            _state.State.Add(id);
            await _state.WriteStateAsync();
            await UpdateParams();
        }

        public async ValueTask<List<long>> GetAllPoisIds()
        {
            return await Task.FromResult(_state.State);
        }

        public async Task<List<PoiState>> GetAllPois()
        {
            return await Task.FromResult(_pois);
        }

        public async ValueTask RemovePoi(long id)
        {
            _state.State.Remove(id);
            await _state.WriteStateAsync();
            await UpdateParams();
        }

        private async ValueTask UpdateParams()
        {
            _pois.Clear();
            if (_state is not { State.Count: > 0 }) return;
            foreach (var id in _state.State)
            {
                _logger.LogTrace("Id of poi ->", id);
                IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>($"poi{id}");
                PoiState poiState = await poiGrain.GetPoiState();
                _pois.Add(poiState);
                _logger.LogInformation($"Poi list information are just loaded to itinerary {this.GetPrimaryKeyString()}");
            };
        }
    }
}
