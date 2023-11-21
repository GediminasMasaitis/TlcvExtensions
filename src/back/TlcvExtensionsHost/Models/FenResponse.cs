using TlcvExtensionsHost.Configs;

namespace TlcvExtensionsHost.Models;

public class FenResponse
{
    public List<EngineConfig?> Engines { get; set; }

    public FenResponse(List<EngineConfig?> engines)
    {
        Engines = engines;
    }
}
