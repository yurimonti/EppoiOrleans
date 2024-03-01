namespace Abstractions
{
    [GenerateSerializer, Immutable]
    public sealed record class ItineraryState
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public List<long> Pois {  get; set; } 
        public double? TimeToVisit { get; set; }
    }
}