using Abstractions;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public class ItineraryCollectionGrain : Grain, IItineraryCollectionGrain
    {
        private readonly IPersistentState<List<long>> _state;
        private readonly ILogger<IItineraryCollectionGrain> _logger;
        private readonly List<ItineraryState> _itineraries;

        public ItineraryCollectionGrain(
            [PersistentState("ItineraryCollection")] IPersistentState<List<long>> state,
            ILogger<IItineraryCollectionGrain> logger)
        {
            _state = state;
            _logger = logger;
            _itineraries = new();
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _ = UpdateParams();
            return base.OnActivateAsync(cancellationToken);
        }

        public async ValueTask AddItinerary(long id)
        {
            _state.State.Add(id);
            await _state.WriteStateAsync();
            await UpdateParams();
        }

        public async ValueTask<List<long>> GetAllItineraryIds()
        {
            return await Task.FromResult(_state.State);
        }

        public async Task<List<ItineraryState>> GetAllItineraries()
        {
            _logger.LogInformation($"itinerary collection grain {this.GetGrainId()} is trying to retrive all itineraries");
            var result = await Task.FromResult(_itineraries);
            _logger.LogInformation($"itinerary collection grain {this.GetGrainId()} retrives all itineraries: {_itineraries}");
            return result;
        }

        public async ValueTask RemoveItinerary(long id)
        {
            _state.State.Remove(id);
            await _state.WriteStateAsync();
            await UpdateParams();
        }

        private async ValueTask UpdateParams()
        {
            _itineraries.Clear();
            if (_state is not { State.Count: > 0 }) return;
            foreach (var id in _state.State)
            {
                _logger.LogTrace("Id of itinerary ->", id);
                IItineraryGrain itineraryGrain = GrainFactory.GetGrain<IItineraryGrain>($"itinerary{id}");
                ItineraryState itineraryState = await itineraryGrain.GetState();
                _itineraries.Add(itineraryState);
                _logger.LogInformation($"Itinerary list information are just loaded to collection {this.GetPrimaryKeyString()}");
            };
        }
    }
}
