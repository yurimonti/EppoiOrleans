namespace Abstractions
{
    [GenerateSerializer, Immutable]
    public sealed class ItineraryState
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public List<long> Pois {  get; set; } 
        public double TimeToVisit { get; set; }
    }
}