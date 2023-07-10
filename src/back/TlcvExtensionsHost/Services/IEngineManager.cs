using TlcvExtensionsHost.Models;
using TlcvExtensionsHost.Services;

namespace TlcvExtensionsHost.Services;

public interface IEngineManager
{
    IList<IEngine> Engines { get; }

    Task RunAsync();
    Task SetFenAsync(string fen);
    IList<EngineInfo> GetCurrentInfos();
}