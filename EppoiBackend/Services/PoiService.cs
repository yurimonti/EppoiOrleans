using Abstractions;
using EppoiBackend.Dtos;
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
            return pois.Select(ConvertToDto).ToList();
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

        public async Task<List<PoiStateDto>> GetPois(List<long> poiIDs)
        {
            List<PoiStateDto> dtos = await GetAllPois();
            var toReturn = dtos.Where(dto => poiIDs.Contains(dto.Id)).ToList();
            return toReturn;
        }

        public async Task<PoiStateDto> UpdatePoi(long poiID, PoiStateDto state)
        {
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{poiID}");
            await poiGrain.SetState(poiID, state.Name, state.Description, state.Address, state.TimeToVisit, state.Coordinate);
            return await _converter.ConvertToDto(poiGrain);
        }
    }
}
