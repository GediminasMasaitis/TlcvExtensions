namespace TlcvExtensionsHost.Configs;

public class EngineConfig
{
    public string Name { get; set; }
    public string Path { get; set; }
    public IList<EngineOption> Options { get; set; }
}