using TlcvExtensionsHost.Models;
using TlcvExtensionsHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace TlcvExtensionsHost.Controllers
{
    public class MainController
    {
        private readonly ILogger<MainController> _logger;
        private readonly IEngineManager _engineManager;

        public MainController(ILogger<MainController> logger, IEngineManager engineManager)
        {
            _logger = logger;
            _engineManager = engineManager;
        }

        [HttpPost]
        [Route("/fen")]
        public async Task<FenResponse> SetFen([FromBody] FenRequest request)
        {
            _logger.LogInformation("FEN: {Fen}", request.Fen);
            await _engineManager.SetFenAsync(request.Fen);

            var response = new FenResponse();
            response.Engines = _engineManager.Engines.Select(x => x.Config).ToList();
            return response;
        }

        [HttpGet]
        [Route("/query")]
        public async Task<QueryResponse> Query()
        {
            var infos = _engineManager.GetCurrentInfos();
            var response = new QueryResponse
            {
                EngineInfos = infos
            };
            return response;
        }
    }
}
