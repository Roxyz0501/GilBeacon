using Dalamud.Configuration;
namespace GilBeacon;
public sealed class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public string? Language { get; set; }
    public bool Enabled { get; set; } = true;
}
