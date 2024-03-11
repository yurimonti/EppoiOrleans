using Microsoft.Extensions.DependencyInjection;
using Orleans.Storage;
using Orleans.TestingHost;
using StackExchange.Redis;

namespace EppoiBackend.Tests
{
    public sealed class ClusterFixture : IDisposable
    {
        public TestCluster Cluster { get; } = new TestClusterBuilder()
        .AddSiloBuilderConfigurator<TestSiloConfigurations>()
        .Build();

        public ClusterFixture() => Cluster.Deploy();

        void IDisposable.Dispose() => Cluster.StopAllSilos();
    }

    file sealed class TestSiloConfigurations : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.UseLocalhostClustering();
            siloBuilder.AddRedisGrainStorageAsDefault(options =>
            {
                options.ConfigurationOptions = new ConfigurationOptions();
                options.DeleteStateOnClear = true;
                options.ConfigurationOptions.EndPoints.Add("localhost", 6379);
            });
        }
        //siloBuilder.AddMemeoryGrainStorageAsDefault();
    }
}
