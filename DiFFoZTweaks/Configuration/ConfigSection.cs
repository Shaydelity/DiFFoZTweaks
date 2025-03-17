using BepInEx.Configuration;

namespace DiFFoZTweaks.Configuration;
public abstract class ConfigSection
{
    public abstract string Section { get; protected set; }

    public ConfigEntry<bool> Enabled { get; protected set; } = null!;

    public abstract void Initialize(ConfigFile config);

    protected void InitializeEnabled(ConfigFile config, bool enabled, string description = "Enable integration with the mod")
    {
        Enabled = config.Bind(Section, "Enabled", enabled, description);
    }
}
