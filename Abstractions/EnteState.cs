﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions
{
    [GenerateSerializer, Immutable]
    public sealed record class EnteState
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string City { get; set; }
        public List<long> PoiIDs { get; set; }

        public UserRole Role { get; private set; } = UserRole.Ente;
    }
}
