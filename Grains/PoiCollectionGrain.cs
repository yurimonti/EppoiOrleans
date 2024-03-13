using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"PoiCollectionGrain: {this.GetGrainId} was just activated");
            await UpdateParams();
            await base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"PoiCollectionGrain: {this.GetGrainId} was just deactivated");
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        /// <summary>
        /// Add a POI id to the list of all existing POI's ids
        /// </summary>
        /// <param name="id">the POI id to add</param>
        /// <returns>A Task</returns>
        public async Task AddPoi(long id)
        {
            _state.State.Add(id);
            await _state.WriteStateAsync();
            await UpdateParams();
            _logger.LogInformation($"PoiCollectionGrain: {this.GetGrainId} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }

        /// <summary>
        /// Retrieve all the POI's ids
        /// </summary>
        /// <returns>A Task that represent the list of ids of each existing POI</returns>
        public async Task<List<long>> GetAllPoisIds()
        {
            _logger.LogInformation($"PoiCollectionGrain: {this.GetGrainId} retrieved its state");
            _logger.LogInformation($"The resulting state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(_state.State)}");
            return await Task.FromResult(_state.State);
        }

        /// <summary>
        /// Retrieve all the POIS
        /// </summary>
        /// <returns>A Task that represent the list of POIStates of each existing POI</returns>
        public async Task<List<PoiState>> GetAllPois()
        {
            _logger.LogInformation($"PoiCollectionGrain: {this.GetGrainId} retrieved its local not persistent state");
            _logger.LogInformation($"The resulting not persistent state for {this.GetPrimaryKeyString()} is {JsonSerializer.Serialize(_pois)}");
            return await Task.FromResult(_pois);
        }

        /// <summary>
        /// Remove a POI id from the list of all POI's ids
        /// </summary>
        /// <param name="id">the id to remove</param>
        /// <returns>A Task</returns>
        public async Task RemovePoi(long id)
        {
            _state.State.Remove(id);
            await _state.WriteStateAsync();
            await UpdateParams();
            _logger.LogInformation($"PoiCollectionGrain: {this.GetGrainId} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        }

        private async Task UpdateParams()
        {
            _pois.Clear();
            if (_state is not { State.Count: > 0 }) return;
            foreach (var id in _state.State)
            {
                _logger.LogTrace("Id of poi ->", id);
                IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>($"poi{id}");
                PoiState poiState = await poiGrain.GetPoiState();
                _pois.Add(poiState);
                _logger.LogInformation($"Poi list information are just loaded to collection {this.GetPrimaryKeyString()}");
            };
        }

        /// <summary>
        /// Check if an id passed is present on the list of all the POI's ids
        /// </summary>
        /// <param name="id">the id to check</param>
        /// <returns>A Task with value true if present, else a Task with value false</returns>
        public async Task<bool> PoiExists(long id)
        {
            return await Task.FromResult(_state.State.Contains(id));
        }
    }
}
