using Abstractions;
using EppoiBackend.Dtos;
using Orleans.Runtime;

namespace EppoiBackend.Services
{
    public class PoiService : IPoiService
    {
        private readonly Random rnd = new();
        private readonly IGrainFactory _grainFactory;
        private static readonly string POI_COLLECTION_ID = "poi-collection";
        private readonly IStateToDtoConverter<IPoiGrain, PoiStateDto> _converter;


        public PoiService(IGrainFactory grainFactory, IStateToDtoConverter<IPoiGrain, PoiStateDto> converter)
        {
            _grainFactory = grainFactory;
            _converter = converter;
        }


        public async Task<PoiState> CreatePoi(PoiStateDto state)
        {
            long idToSet = rnd.NextInt64();
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{idToSet}");
            await poiGrain.SetState(idToSet, state.Name, state.Description, state.Address, state.TimeToVisit, state.Coordinate);
            await poiCollectionGrain.AddPoi(idToSet);
            return await poiGrain.GetPoiState();
        }

        public async Task<PoiStateDto> GetAPoi(long id)
        {
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{id}");
            //PoiState state = await poiGrain.GetPoiState();
            return await _converter.ConvertToDto(poiGrain);
        }

        public async Task<List<PoiStateDto>> GetAllPois()
        {
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            List<PoiState> pois = await poiCollectionGrain.GetAllPois();
            List<PoiStateDto> dtos = pois.Select(ConvertToDto).ToList();
            return dtos;
        }
        private PoiStateDto ConvertToDto(PoiState poiState)
        {
            return new PoiStateDto
            {
                Name = poiState.Name,
                Description = poiState.Description,
                Coordinate = poiState.Coordinate,
                Address = poiState.Address,
                Id = poiState.Id,
                TimeToVisit = poiState.TimeToVisit
            };
        }

    }
}
