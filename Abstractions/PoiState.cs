using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    [GenerateSerializer, Immutable]
    public sealed class PoiState
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public (double Lat,double Lon) Coordinate { get; set; }
        public double TimeToVisit { get; set; }
        public string Address { get; set; }
    }
}
