﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    [GenerateSerializer, Immutable]
    public record class Coordinate
    {
        public double Lat {  get; set; }
        public double Lon {  get; set; }
    }
}
