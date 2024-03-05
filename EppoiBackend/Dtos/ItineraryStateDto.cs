using Abstractions;
using System.Text.Json.Serialization;

namespace EppoiBackend.Dtos
{
    public record class ItineraryStateDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double TimeToVisit { get; set; }
        public List<PoiStateDto> Pois { get; set; }
    }
}
