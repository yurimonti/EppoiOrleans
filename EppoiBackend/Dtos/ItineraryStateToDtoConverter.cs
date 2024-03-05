using Abstractions;

namespace EppoiBackend.Dtos
{
    public class ItineraryStateToDtoConverter : IStateToDtoConverter<IItineraryGrain, ItineraryStateDto>
    {
        public async Task<ItineraryStateDto> ConvertToDto(IItineraryGrain grain)
        {
            //var state = await grain.GetState();
            //List<PoiState> pois = await grain.GetPois();
            //return new ItineraryStateDto
            //{
            //    Name = state.Name,
            //    Id = state.Id,
            //    Description = state.Description,
            //    TimeToVisit = (double)state.TimeToVisit,
            //    Pois = pois
            //};
            throw new NotImplementedException();
        }
    }
}
