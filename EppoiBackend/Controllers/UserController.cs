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
        private readonly IGrainFactory _grainFactory;
        private readonly ILogger<UserController> _logger;

        public UserController(IPoiService poiService,
            IGrainFactory grainFactory,
            IItineraryService itineraryService,
            ILogger<UserController> logger)
        {
            _poiService = poiService;
            _itineraryService = itineraryService;
            _grainFactory = grainFactory;
            _logger = logger;
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
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            await userGrain.SetState(userInfo.Username, new List<long>());
            UserState state = await userGrain.GetState();
            return Ok(state);
        }

        [HttpGet("{id}/poi")]
        public async ValueTask<IActionResult> GetPois(Guid id)
        {
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            UserState state = await userGrain.GetState();
            if (IsAUser(state, id))
            {
                var poiStates = await userGrain.GetPois();
                var toReturn = _poiService.ConvertToDto(poiStates);
                return Ok(toReturn);
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/poi/{poiId}")]
        public async ValueTask<IActionResult> GetPoiState(Guid id, string poiId)
        {
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                try
                {
                    var poiState = await userGrain.GetPoiState(long.Parse(poiId));
                    var toReturn = _poiService.ConvertToDto(poiState);
                    return Ok(toReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/itinerary")]
        public async ValueTask<IActionResult> GetItineraries(Guid id, [FromQuery] bool mineOnly)
        {
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            UserState state = await userGrain.GetState();
            Func<ItineraryState, bool> function = (it => state.ItineraryIDs.Contains(it.Id));
            if (IsAUser(state, id))
            {
                List<ItineraryState> states = await userGrain.GetItineraries(mineOnly);
                List<ItineraryStateDto> toReturn = await _itineraryService.ConvertToDto(states);
                return Ok(toReturn);
            }
            else return Unauthorized();
        }

        [HttpGet("{id}/itinerary/{itineraryId}")]
        public async ValueTask<IActionResult> GetItinerayState(Guid id, string itineraryId)
        {
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                try
                {
                    var itinerary = await userGrain.GetItineraryState(long.Parse(itineraryId));
                    var toReturn = await _itineraryService.ConvertToDto(itinerary);
                    return Ok(toReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else return Unauthorized();
        }

        [HttpPost("{id}/itinerary/")]
        public async ValueTask<IActionResult> CreateItinerary([FromBody] ItineraryState state, Guid id)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                try
                {
                    ItineraryState stateToReturn = await userGrain.CreateItinerary(state);
                    userState.ItineraryIDs.Add(stateToReturn.Id);
                    await userGrain.SetState(userState.Username, userState.ItineraryIDs);
                    var toReturn = await _itineraryService.ConvertToDto(stateToReturn);
                    return Ok(toReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            else return Unauthorized();
        }


        [HttpPut("{id}/itinerary/{itineraryId}")]
        public async ValueTask<IActionResult> UpdateItinerary([FromBody] ItineraryState state, Guid id, string itineraryId)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                try
                {
                    ItineraryState stateToReturn = await userGrain.UpdateItinerary(state,long.Parse(itineraryId));
                    var toReturn = await _itineraryService.ConvertToDto(stateToReturn);
                    return Ok(toReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else return Unauthorized();
        }

        [HttpDelete("{id}/itinerary/{itineraryId}")]
        public async ValueTask<IActionResult> DeleteItinerary(Guid id, string itineraryId)
        {
            //Should be present a control over the ente city and the poi city (Open Route Service)
            IUserGrain userGrain = _grainFactory.GetGrain<IUserGrain>(id);
            UserState userState = await userGrain.GetState();
            if (IsAUser(userState, id))
            {
                try
                {
                    await userGrain.RemoveItinerary(long.Parse(itineraryId));
                    return Ok("itinerary successfully deleted");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else return Unauthorized();
        }
    }
}
