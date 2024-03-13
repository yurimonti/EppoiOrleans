using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.Text.Json;

namespace Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly IPersistentState<UserState> _state;
        private readonly ILogger<IPoiGrain> _logger;
        private static readonly string ITINERARY_COLLECTION_ID = "itinerary-collection";
        private static readonly string POI_COLLECTION_ID = "poi-collection";
        private IItineraryCollectionGrain _itineraryCollectionGrain;

        public UserGrain([PersistentState("User")] IPersistentState<UserState> state, ILogger<IPoiGrain> logger)
        {
            _state = state;
            _logger = logger;
            _itineraryCollectionGrain = GrainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"UserGrain: {Guid.Parse(this.GetPrimaryKeyString())} was just activated");
            return base.OnActivateAsync(cancellationToken);
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"UserGrain: {Guid.Parse(this.GetPrimaryKeyString())} was just deactivated");
            return base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task<UserState> GetState()
        {
            UserState state = _state.State;
            _logger.LogInformation($"UserGrain: {Guid.Parse(this.GetPrimaryKeyString())} retrieved its state");
            _logger.LogInformation($"The resulting state for {Guid.Parse(this.GetPrimaryKeyString())} is {JsonSerializer.Serialize(state)}");
            return await ValueTask.FromResult(state);
        }

        public async Task SetState(string username, List<long> itineraryIDs)
        {
            _state.State = new() { Id = Guid.Parse(this.GetPrimaryKeyString()), Username = username, ItineraryIDs = itineraryIDs };
            await _state.WriteStateAsync();
            _logger.LogInformation($"The state {JsonSerializer.Serialize(_state.State)} is setted to {Guid.Parse(this.GetPrimaryKeyString())}");
            _logger.LogInformation($"UserGrain: {Guid.Parse(this.GetPrimaryKeyString())} setted its state");
        }

        public async Task<ItineraryState> CreateItinerary(ItineraryState toCreate)
        {
            long itineraryId = Random.Shared.NextInt64();
            if (await _itineraryCollectionGrain.ItineraryExists(itineraryId)) throw new Exception($"$Itinerary with id:{itineraryId} already exists");
            IItineraryGrain itineraryGrain = GrainFactory.GetGrain<IItineraryGrain>(itineraryId);
            ItineraryState itineraryState = await itineraryGrain.SetState(toCreate.Name, toCreate.Description, toCreate.Pois);
            await _itineraryCollectionGrain.AddItinerary(itineraryId);
            _state.State.ItineraryIDs.Add(itineraryId);
            await SetState(_state.State.Username, _state.State.ItineraryIDs);
            return itineraryState;
        }

        public async Task<ItineraryState> UpdateItinerary(ItineraryState newState, long itineraryToUpdate)
        {
            if (!await _itineraryCollectionGrain.ItineraryExists(itineraryToUpdate))
                throw new ArgumentException($"Itinerary with id:{itineraryToUpdate} doesn't exist");
            if (!_state.State.ItineraryIDs.Contains(itineraryToUpdate)) throw new Exception("You cannot update this Itinerary");
            IItineraryGrain itineraryGrain = GrainFactory.GetGrain<IItineraryGrain>(itineraryToUpdate);
            await itineraryGrain.SetState(newState.Name, newState.Description, newState.Pois);
            return newState;
        }

        public async Task RemoveItinerary(long toRemove)
        {
            if (!await _itineraryCollectionGrain.ItineraryExists(toRemove)) throw new ArgumentException($"Itinerary with id:{toRemove} doesn't exist");
            if (!_state.State.ItineraryIDs.Contains(toRemove)) throw new Exception("You cannot delete this Itinerary");
            IItineraryGrain itineraryGrain = GrainFactory.GetGrain<IItineraryGrain>(toRemove);
            await itineraryGrain.ClearState();
            _state.State.ItineraryIDs.Remove(toRemove);
            await SetState(_state.State.Username, _state.State.ItineraryIDs);
            await _itineraryCollectionGrain.RemoveItinerary(toRemove);
        }

        public async Task<List<PoiState>> GetPois()
        {
            return await GrainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID).GetAllPois();
        }

        public async Task<PoiState> GetPoiState(long toRetrieve)
        {
            if (!await GrainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID).PoiExists(toRetrieve)) 
                throw new ArgumentException($"Poi with id:{toRetrieve} doesn't exist");
            IPoiGrain poiGrain = GrainFactory.GetGrain<IPoiGrain>(toRetrieve);
            return await poiGrain.GetState();
        }

        public async Task<List<ItineraryState>> GetItineraries(bool mineOnly)
        {
            List<ItineraryState> itineraries = await _itineraryCollectionGrain.GetAllItineraries();
            return mineOnly ? itineraries.Where(it => _state.State.ItineraryIDs.Contains(it.Id)).ToList() : itineraries ;
        }

        public async Task<ItineraryState> GetItineraryState(long toRetrieve)
        {
            if (!await _itineraryCollectionGrain.ItineraryExists(toRetrieve))
                throw new ArgumentException($"Itinerary with id:{toRetrieve} doesn't exist");
            IItineraryGrain itineraryGrain = GrainFactory.GetGrain<IItineraryGrain>(toRetrieve);
            return await itineraryGrain.GetState();
        }

        //private Guid GetPoiIdFromGrainStringKey()
        //{
        //    return Guid.Parse(this.GetPrimaryKeyString().Split("/")[1]);
        //}
    }
}
