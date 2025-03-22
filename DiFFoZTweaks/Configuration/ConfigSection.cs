using BepInEx.Configuration;

namespace DiFFoZTweaks.Configuration;
public abstract class ConfigSection
{
    private readonly ConfigFile m_Config;

    protected ConfigSection(ConfigFile config, string section, bool isEnabled = true, string enabledDescription = "Enable integration with the mod")
    {
        m_Config = config;
        Section = section;
        Enabled = config.Bind(section, "Enabled", isEnabled, enabledDescription);
        Initialize(config);
    }

    public string Section { get; }

    public ConfigEntry<bool> Enabled { get; protected set; } = null!;

    protected abstract void Initialize(ConfigFile config);

    protected ConfigEntryCheck<T> Bind<T>(string key, T defaultValue, string description)
    {
        var entry = m_Config.Bind(Section, key, defaultValue, description);
        return new ConfigEntryCheck<T>(Enabled, entry);
    }
}
