using Abstractions;
using EppoiBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using static EppoiBackend.Controllers.EnteController;

namespace EppoiBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private static readonly string POI_COLLECTION_ID = "poi-collection";
        private static readonly string ITINERARY_COLLECTION_ID = "itinerary-collection";
        private readonly IGrainFactory _grainFactory;

        public TestController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpPost]
        public async ValueTask<IActionResult> Init()
        {
            //intialization of ente and Pois
            Guid enteId = new("81385174-3ca5-4851-9dd1-80264df1e903");
            long poiId1 = 189854349937223524;
            long poiId2 = 680854246637223527;
            IPoiGrain poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{poiId1}");
            await poiGrain.SetState(poiId1, "poi1", "description1", "via Roma 10", 10.20, new Coordinate { Lat = 43.122323, Lon = 13.456564 });
            poiGrain = _grainFactory.GetGrain<IPoiGrain>($"poi{poiId2}");
            await poiGrain.SetState(poiId2, "poi2", "description2", "via Roma 12", 12, new Coordinate { Lat = 43.24356, Lon = 13.890675 });
            IPoiCollectionGrain poiCollectionGrain = _grainFactory.GetGrain<IPoiCollectionGrain>(POI_COLLECTION_ID);
            await poiCollectionGrain.AddPoi(poiId1);
            await poiCollectionGrain.AddPoi(poiId2);
            var enteGrain = _grainFactory.GetGrain<IEnteGrain>($"ente{enteId}");
            await enteGrain.SetState(enteId, "ente-camerino", "Camerino", new List<long>() { poiId1, poiId2 });
            //intialization of user and itineraries
            Guid userId = new("642d23f2-657d-469b-8268-4002ab2b37d7");
            long itineraryId = 7489405095330777579;
            IItineraryGrain itineraryGrain = _grainFactory.GetGrain<IItineraryGrain>($"itinerary{itineraryId}");
            await itineraryGrain.SetState(itineraryId, "poi1", "description1", new List<long>() { poiId1, poiId2 });
            IItineraryCollectionGrain itineraryCollectionGrain = _grainFactory.GetGrain<IItineraryCollectionGrain>(ITINERARY_COLLECTION_ID);
            await itineraryCollectionGrain.AddItinerary(itineraryId);
            var userGrain = _grainFactory.GetGrain<IUserGrain>($"user{userId}");
            await userGrain.SetState(userId, "utente", [itineraryId]);
            //return OK
            Console.WriteLine("END OF TEST INIT CALL ------------------------------ END OF TEST INIT CALL");
            return Ok("grain and database initialized");
        }
    }
}
