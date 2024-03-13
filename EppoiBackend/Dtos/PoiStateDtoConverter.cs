using Abstractions;

namespace EppoiBackend.Dtos
{
    public class PoiStateDtoConverter : IStateToDtoConverter<IPoiGrain, PoiStateDto>
    {
        public async Task<PoiStateDto> ConvertToDto(IPoiGrain grain)
        {
            //var state = await grain.GetState();
            //return new PoiStateDto
            //{
            //    Name = state.Name,
            //    Description = state.Description,
            //    Coords = state.Coords,
            //    Address = state.Address,
            //    Id = state.Id,
            //    TimeToVisit = state.TimeToVisit
            //};
            throw new NotImplementedException();
        }
    }
}
