using Abstractions;
using EppoiBackend.Services;
using Grains;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Orleans;
using Xunit.Sdk;

namespace EppoiBackend.Tests;

public class PoiGrainTests
{
    //private readonly Random _random = new Random();
    private readonly PoiState expected = new() { Id = 234321942, Name = "name", Description = "d", Address = "F", TimeToVisit = 12.4, Coords = new() { Lat = 12.5465, Lon = 13.4356 } };
    //private IPoiCollectionGrain _collectionGrain;
    //private IGrainFactory _grainFactory;

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async void TestGetAPoi(bool exists)
    {
        //ARRANGE
        long idToRetrieve = 234321942;
        //Poi Grain
        var poiGrain = Substitute.For<IPoiGrain>();
        PoiState expectedValue = new() { Id = idToRetrieve, Name = "name", Description = "d", Address = "F", TimeToVisit = 12.4, Coords = new() { Lat = 12.5465, Lon = 13.4356 } };
        poiGrain.GetPoiState().Returns(Task.FromResult(expectedValue));
        //Poi Collection
        var poiCollectionGrain = Substitute.For<IPoiCollectionGrain>();
        poiCollectionGrain.PoiExists(idToRetrieve).Returns(Task.FromResult(exists));
        //Grain Factory
        var grainFactory = Substitute.For<IGrainFactory>();
        grainFactory.GetGrain<IPoiGrain>($"poi{idToRetrieve}").Returns(poiGrain);
        grainFactory.GetGrain<IPoiCollectionGrain>(Arg.Any<string>()).Returns(poiCollectionGrain);
        //Poi Service
        IPoiService service = new PoiService(grainFactory);
        //ACT and ASSERT
        if(exists)
        {
            PoiState stateToRetrieve = await service.GetAPoi(idToRetrieve);
            Assert.Equal(expectedValue, stateToRetrieve);
        }
        else await Assert.ThrowsAsync<ArgumentException>( () => service.GetAPoi(idToRetrieve));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async void TestCreateAPoi(bool exists)
    {
        var poiCollectionGrain = Substitute.For<IPoiCollectionGrain>();
        poiCollectionGrain.PoiExists(Arg.Any<long>()).Returns(Task.FromResult(exists));
        var poiGrain = Substitute.For<IPoiGrain>();
        poiGrain.GetPoiState().Returns(Task.FromResult(expected));
        var grainFactory = Substitute.For<IGrainFactory>();
        grainFactory.GetGrain<IPoiGrain>(Arg.Any<string>()).Returns(poiGrain);
        grainFactory.GetGrain<IPoiCollectionGrain>(Arg.Any<string>()).Returns(poiCollectionGrain);
        //Poi Collection
        IPoiService service = new PoiService(grainFactory);
        if(exists) await Assert.ThrowsAsync<Exception>(()=>service.CreatePoi(expected));
        else
        {
            PoiState actual = await service.CreatePoi(expected);
            actual.Id = expected.Id;
            Assert.Equal(expected, actual);
        }
    }
}