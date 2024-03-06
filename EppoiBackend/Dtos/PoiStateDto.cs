using Abstractions;

namespace EppoiBackend.Dtos
{
    public record class PoiStateDto
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public Coordinate Coords { get; set; }
        public double TimeToVisit { get; set; }
    }
}
