using TlcvExtensionsHost.Configs;
using TlcvExtensionsHost.Models;

namespace TlcvExtensionsHost.Services;

public class EngineManager
{
    private readonly IServiceProvider _provider;
    private readonly ServiceConfig _config;

    private readonly List<Task> _engineTasks;

    public List<Engine> Engines { get; }

    public EngineManager(IServiceProvider provider, ServiceConfig config)
    {
        _provider = provider;
        _config = config;

        Engines = new List<Engine>(config.Engines.Count);
        _engineTasks = new List<Task>(config.Engines.Count);
    }

    public void Run()
    {
        foreach (var engineConfig in _config.Engines)
        {
            var engine = _provider.GetRequiredService<Engine>();
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

    public List<EngineInfo> GetCurrentInfos()
    {
        var infos = new List<EngineInfo>(Engines.Count);
        foreach (Engine engine in Engines)
        {
            if (engine.CurrentEngineInfo is not null)
            {
                infos.Add(engine.CurrentEngineInfo);
            }
        }
        return infos;
    }
}
