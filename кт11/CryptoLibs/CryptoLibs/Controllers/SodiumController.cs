using CryptoLibs.Services;

namespace CryptoLibs.Controllers
{
    public class SodiumController : CryptoControllerBase
    {
        private readonly SodiumService _service;

        public SodiumController(SodiumService service)
        {
            _service = service;
        }

        protected override ICryptoProvider Provider => _service;
    }
}
