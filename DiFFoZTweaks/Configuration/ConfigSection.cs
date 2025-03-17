using BepInEx.Configuration;

namespace DiFFoZTweaks.Configuration;
public abstract class ConfigSection
{
    public abstract string Section { get; protected set; }

    public ConfigEntry<bool> Enabled { get; protected set; } = null!;

    public virtual void Initialize(ConfigFile config)
    {
        Enabled = config.Bind(Section, "Enabled", false, "Enable integration");
    }
}
