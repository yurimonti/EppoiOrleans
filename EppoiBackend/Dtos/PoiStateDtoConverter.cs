using Abstractions;

namespace EppoiBackend.Dtos
{
    public class PoiStateDtoConverter : IStateToDtoConverter<IPoiGrain, PoiStateDto>
    {
        public async Task<PoiStateDto> ConvertToDto(IPoiGrain grain)
        {
            var state = await grain.GetPoiState();
            return new PoiStateDto
            {
                Name = state.Name,
                Description = state.Description,
                Coordinate = state.Coordinate,
                Address = state.Address,
                Id = state.Id,
                TimeToVisit = state.TimeToVisit
            };
        }
    }
}
