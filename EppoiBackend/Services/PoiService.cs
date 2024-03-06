using Abstractions;
using EppoiBackend.Dtos;
using Grains;
using Orleans.Runtime;
using System.Text.Json;

namespace EppoiBackend.Services
{
    //TODO: add control on the existence of poi into the poi collection grain
    public class PoiService : IPoiService
    {
        private readonly Random rnd = new();
        private readonly IGrainFactory _grainFactory;
        private static readonly string POI_COLLECTION_ID = "poi-collection";
        private static readonly string ITINERARY_COLLECTION_ID = "itinerary-collection";
        private readonly IStateToDtoConverter<IPoiGrain, PoiStateDto> _converter;


        public PoiService(IGrainFactory grainFactory, IStateToDtoConverter<IPoiGrain, PoiStateDto> converter)
        {
            _grainFactory = grainFactory;
            _converter = converter;
        }

        public async Task<PoiState> CreatePoi(PoiStateDto state)
        {
            long idToSet = rnd.NextInt64();
            int tries = 0;
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            while (await poiCollectionGrain.PoiExists(idToSet))
            {
                if (tries >= 10) throw new Exception("Lot of tries to find a valid id for a new itinerary");
                idToSet = rnd.NextInt64();
                tries++;
            }
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{idToSet}");
            await poiGrain.SetState(idToSet, state.Name, state.Description, state.Address, state.TimeToVisit, state.Coords);
            await poiCollectionGrain.AddPoi(idToSet);
            return await poiGrain.GetPoiState();
        }

        public async Task<PoiStateDto> GetAPoi(long id)
        {
            await ThrowExceptionIfPoiNotExists(_grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID), id);
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            return await _converter.ConvertToDto(poiGrain);
        }

        public async Task<List<PoiStateDto>> GetAllPois()
        {
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            List<PoiState> pois = await poiCollectionGrain.GetAllPois();
            return pois.Select(ConvertToDto).ToList();
        }

        private PoiStateDto ConvertToDto(PoiState poiState)
        {
            return new PoiStateDto
            {
                Name = poiState.Name,
                Description = poiState.Description,
                Coords = poiState.Coords,
                Address = poiState.Address,
                Id = poiState.Id,
                TimeToVisit = poiState.TimeToVisit
            };
        }

        public async Task<List<PoiStateDto>> GetPois(List<long> poiIDs)
        {
            List<PoiStateDto> dtos = await GetAllPois();
            var toReturn = dtos.Where(dto => poiIDs.Contains(dto.Id)).ToList();
            return toReturn;
        }

        public async Task<PoiStateDto> UpdatePoi(long poiID, PoiStateDto state)
        {
            await ThrowExceptionIfPoiNotExists(_grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID), poiID);
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{poiID}");
            await poiGrain.SetState(poiID, state.Name, state.Description, state.Address, state.TimeToVisit, state.Coords);
            return await _converter.ConvertToDto(poiGrain);
        }

        public async Task DeletePoi(long id)
        {
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            await ThrowExceptionIfPoiNotExists(poiCollectionGrain,id);
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            await poiGrain.ClearState();
            await poiCollectionGrain.RemovePoi(id);
            await itineraryCollectionGrain.RemoveItinerariesWithPoi(id);
        }

        private async Task ThrowExceptionIfPoiNotExists(IPoiCollectionGrain collectionGrain, long idToCheck)
        {
            if (!await collectionGrain.PoiExists(idToCheck)) throw new ArgumentException($"Poi with id:{idToCheck} doesn't exist");
        }

    }
}
