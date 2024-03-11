using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EppoiBackend.Tests
{
    [CollectionDefinition(Name)]
    public sealed class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
        public const string Name = nameof(ClusterCollection);
    }
}
