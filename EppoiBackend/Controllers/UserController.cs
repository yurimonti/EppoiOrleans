using Abstractions;
using EppoiBackend.Dtos;
using EppoiBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace EppoiBackend.Controllers
{
    [ApiController]
    [Route("api/user/")]
    public class UserController : ControllerBase
    {
        private readonly IPoiService _poiService;
        private readonly IItineraryService _itineraryService;
        private readonly IStateToDtoConverter<IPoiGrain, PoiStateDto> _poiConverter;
        private readonly IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> _itineraryConverter;
        private readonly IGrainFactory _grainFactory;

        public UserController(IPoiService poiService, IStateToDtoConverter<IPoiGrain, PoiStateDto> converter,
            IGrainFactory grainFactory, IStateToDtoConverter<IItineraryGrain, ItineraryStateDto> itineraryConverter,
            IItineraryService itineraryService)
        {
            _poiService = poiService;
            _itineraryService = itineraryService;
            _poiConverter = converter;
            _itineraryConverter = itineraryConverter;
            _grainFactory = grainFactory;
        }

        public record UserInfo
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }
        }

        private bool IsAUser(UserState userState, Guid id) => userState.Id == id && userState.Role == UserRole.User;

        [HttpPost("signup")]
        public async ValueTask<IActionResult> SignUp([FromBody] UserInfo userInfo)
        {
            Guid id = Guid.NewGuid();
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>($"user{id}");
            await userGrain.SetState(id, userInfo.Username, new List<long>());
            UserState state = await userGrain.GetState();
            return Ok(state);
        }

        [HttpGet("{id}/poi")]
        public async ValueTask<IActionResult> GetPois(Guid id)
        {
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>($"user{id}");
            UserState state = await userGrain.GetState();
            if (IsAUser(state, id))
            {
                var toReturn = await _poiService.GetAllPois();
                return Ok(toReturn);
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/poi/{poiId}")]
        public async ValueTask<IActionResult> GetPoiState(Guid id, string poiId)
        {
            //TODO:returns ente pois instead the whole state of an ente
            //PoiStateDto stateDtos = await _poiService.GetAPoi(long.Parse(poiId));
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>($"user{id}");
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                var poi = await _poiService.GetAPoi(long.Parse(poiId));
                return Ok(poi);
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/itinerary")]
        public async ValueTask<IActionResult> GetItineraries(Guid id)
        {
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>($"user{id}");
            UserState state = await userGrain.GetState();
            Func<ItineraryStateDto, bool> function = (dto => state.ItineraryIDs.Contains(dto.Id));
            if (IsAUser(state, id))
            {
                var toReturn = await _itineraryService.GetAllItineraries(function);
                return Ok(toReturn);
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/itinerary/{itineraryId}")]
        public async ValueTask<IActionResult> GetItinerayState(Guid id, string itineraryId)
        {
            //TODO:returns ente pois instead the whole state of an ente
            //PoiStateDto stateDtos = await _poiService.GetAPoi(long.Parse(poiId));
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>($"user{id}");
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                try
                {
                    var itinerary = await _itineraryService.GetAnItinerary(long.Parse(itineraryId));
                    return Ok(itinerary);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else return Unauthorized();
        }

        [HttpPost("{id}/itinerary/")]
        public async ValueTask<IActionResult> CreatePoi([FromBody] ItineraryStateDto state, Guid id)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>($"user{id}");
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                ItineraryState stateToReturn = await _itineraryService.CreateItinerary(state);
                userState.ItineraryIDs.Add(stateToReturn.Id);
                await userGrain.SetState(userState.Username, userState.ItineraryIDs);
                return Ok(stateToReturn);
            }
            else return Unauthorized();
        }


        [HttpPut("{id}/itinerary/{itineraryId}")]
        public async ValueTask<IActionResult> UpdatePoi([FromBody] ItineraryStateDto state, Guid id, string itineraryId)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>($"user{id}");
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                ItineraryStateDto stateToReturn = await _itineraryService.UpdateItinerary(long.Parse(itineraryId), state);
                return Ok(stateToReturn);
            }
            else return Unauthorized();
        }

    }
}
