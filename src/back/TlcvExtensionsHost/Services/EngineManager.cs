using TlcvExtensionsHost.Configs;
using TlcvExtensionsHost.Models;

namespace TlcvExtensionsHost.Services
{
    public class EngineManager : IEngineManager
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<EngineManager> _logger;
        private readonly ServiceConfig _config;

        private readonly IList<Task> _engineTasks;

        public IList<IEngine> Engines { get; }

        public EngineManager(IServiceProvider provider, ILogger<EngineManager> logger, ServiceConfig config)
        {
            _provider = provider;
            _logger = logger;
            _config = config;

            Engines = new List<IEngine>();
            _engineTasks = new List<Task>();
        }

        public async Task RunAsync()
        {
            foreach (var engineConfig in _config.Engines)
            {
                var engine = _provider.GetRequiredService<IEngine>();
                var engineTask = engine.RunAsync(engineConfig);
                Engines.Add(engine);
                _engineTasks.Add(engineTask);
            }
        }

        public async Task SetFenAsync(string fen)
        {
            foreach (var engine in Engines)
            {
                await engine.SetFenAsync(fen);
            }
        }

        public IList<EngineInfo> GetCurrentInfos()
        {
            var infos = new List<EngineInfo>();
            foreach (var engine in Engines)
            {
                infos.Add(engine.CurrentEngineInfo);
            }
            return infos;
        }
    }
}
