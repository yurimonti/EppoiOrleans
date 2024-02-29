using Abstractions;

namespace EppoiBackend.Services
{
    public class PoiService
    {
        private readonly Random rnd = new Random();
        public PoiState CreatePoi(string name, string address)
        {
            return new PoiState { Address = address, Name = name, Id = rnd.NextInt64() };
        }
    }
}
