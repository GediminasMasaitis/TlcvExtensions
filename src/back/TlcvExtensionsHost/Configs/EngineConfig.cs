namespace TlcvExtensionsHost.Configs;

public class EngineConfig
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public IList<EngineOption> Options { get; set; } = new List<EngineOption>();
}