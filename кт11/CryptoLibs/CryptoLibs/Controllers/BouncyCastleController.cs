using CryptoLibs.Services;

namespace CryptoLibs.Controllers
{
    public class BouncyCastleController : CryptoControllerBase
    {
        private readonly BouncyCastleAesService _service;

        public BouncyCastleController(BouncyCastleAesService service)
        {
            _service = service;
        }

        protected override ICryptoProvider Provider => _service;
    }
}
