using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Text.Json;

namespace Grains
{
    public class EnteGrain : Grain, IEnteGrain
    {
        private readonly ILogger<IEnteGrain> _logger;
        private readonly IPersistentState<EnteState> _state;
        private IPoiCollectionGrain _poiCollectionGrain;
        private static readonly string POI_COLLECTION_ID = "poi-collection";
        private static readonly string ITINERARY_COLLECTION_ID = "itinerary-collection";


        public EnteGrain(ILogger<IEnteGrain> logger, [PersistentState("Ente")] IPersistentState<EnteState> state)
        {
            _logger = logger;
            _state = state;
            _poiCollectionGrain = GrainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
        }
        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"EnteGrain: {Guid.Parse(this.GetPrimaryKeyString())} was just activated");
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"EnteGrain: {Guid.Parse(this.GetPrimaryKeyString())} was just deactivated");
            return base.OnDeactivateAsync(reason, cancellationToken);
        }


        public async ValueTask<EnteState> GetState()
        {
            _logger.LogInformation($"EnteGrain: {Guid.Parse(this.GetPrimaryKeyString())} retrieved its state");
            _logger.LogInformation($"The resulting state for {Guid.Parse(this.GetPrimaryKeyString())} is {JsonSerializer.Serialize(_state.State)}");
            return await Task.FromResult(_state.State);
        }

        public async ValueTask SetState(string username, string city, List<long> poiIDs)
        {
            _state.State.Id = Guid.Parse(this.GetPrimaryKeyString());
            _state.State.City = city;
            _state.State.PoiIDs = poiIDs;
            _state.State.Username = username;
            await _state.WriteStateAsync();
            _logger.LogInformation($"EnteGrain: {Guid.Parse(this.GetPrimaryKeyString())} setted its state");
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {Guid.Parse(this.GetPrimaryKeyString())}");
        }

        public async Task<PoiState> GetPoiState(long toRetrieve)
        {
            if (!await _poiCollectionGrain.PoiExists(toRetrieve)) throw new ArgumentException($"Poi with id:{toRetrieve} doesn't exist");
            IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>(toRetrieve);
            return await poiGrain.GetState();
        }

        public async Task<PoiState> CreatePoi(PoiState toCreate)
        {
            long poiId = Random.Shared.NextInt64();
            if (await _poiCollectionGrain.PoiExists(poiId)) throw new Exception($"Poi with id:{poiId} already exists");
            IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>(poiId);
            PoiState poiState = await poiGrain.SetState(toCreate.Name, toCreate.Description, toCreate.Address, toCreate.TimeToVisit, toCreate.Coords);
            await _poiCollectionGrain.AddPoi(poiId);
            _state.State.PoiIDs.Add(poiId);
            await SetState(_state.State.Username, _state.State.City, _state.State.PoiIDs);
            return poiState;
        }

        public async Task<PoiState> UpdatePoi(PoiState newState, long poiToUpdate)
        {
            if (!await _poiCollectionGrain.PoiExists(poiToUpdate)) throw new ArgumentException($"Poi with id:{poiToUpdate} doesn't exist");
            if (!_state.State.PoiIDs.Contains(poiToUpdate)) throw new Exception("You cannot update this Poi");
            IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>(poiToUpdate);
            await poiGrain.SetState(newState.Name,newState.Description,newState.Address,newState.TimeToVisit,newState.Coords);
            return newState;
        }

        public async Task RemovePoi(long toRemove)
        {
            if (!await _poiCollectionGrain.PoiExists(toRemove)) throw new ArgumentException($"Poi with id:{toRemove} doesn't exist");
            if (!_state.State.PoiIDs.Contains(toRemove)) throw new Exception("You cannot delete this Poi");
            IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>(toRemove);
            await poiGrain.ClearState();
            _state.State.PoiIDs.Remove(toRemove);
            await SetState(_state.State.Username, _state.State.City, _state.State.PoiIDs);
            await _poiCollectionGrain.RemovePoi(toRemove);
            IItineraryCollectionGrain itineraryCollectionGrain = GrainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await itineraryCollectionGrain.RemoveItinerariesWithPoi(toRemove);
        }

        //public async ValueTask SetState(Guid id,string username, string city, List<long> poiIDs)
        //{
        //    _state.State.Id = id;
        //    _state.State.City = city;
        //    _state.State.PoiIDs = poiIDs;
        //    _state.State.Username = username;
        //    await _state.WriteStateAsync();
        //    _logger.LogInformation($"EnteGrain: {this.GetPrimaryKeyString()} setted its state");
        //    _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {this.GetPrimaryKeyString()}");
        //}

        public async Task<List<PoiState>> GetPois(bool mineOnly)
        {
            List<PoiState> pois = await _poiCollectionGrain.GetAllPois();
            return mineOnly ? pois.Where(poi => _state.State.PoiIDs.Contains(poi.Id)).ToList() : pois;
        }

        //private Guid GetPoiIdFromGrainStringKey()
        //{
        //    return Guid.Parse(this.GetPrimaryKeyString().Split("/")[1]);
        //}
    }
}
