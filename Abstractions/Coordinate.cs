namespace Abstractions
{
    [GenerateSerializer, Immutable]
    public record class Coordinate
    {
        public double Lat {  get; set; }
        public double Lon {  get; set; }
    }
}
