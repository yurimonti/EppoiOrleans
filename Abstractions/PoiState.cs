namespace Abstractions
{
    [GenerateSerializer, Immutable]
    public sealed record class PoiState
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public Coordinate Coords { get; set; }
        public double TimeToVisit { get; set; }
        public string Address { get; set; }
    }
}
