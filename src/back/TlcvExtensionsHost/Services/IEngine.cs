using TlcvExtensionsHost.Configs;
using TlcvExtensionsHost.Models;

namespace TlcvExtensionsHost.Services;

public interface IEngine
{
    EngineInfo CurrentEngineInfo { get; }
    EngineConfig Config { get; }

    Task RunAsync(EngineConfig config);
    Task SetFenAsync(string fen);
}