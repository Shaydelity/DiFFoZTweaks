using BepInEx.Configuration;

namespace DiFFoZTweaks.Patcher.Configuration.Configs;
public class MoreCompanyConfig : ConfigSection
{
    public MoreCompanyConfig(ConfigFile config)
        : base(config, "Mods - MoreCompany", true)
    {
    }

    public ConfigEntryCheck<int> CosmeticLimit { get; private set; } = null!;

    protected override void Initialize(ConfigFile config)
    {
        CosmeticLimit = config.Bind(Enabled, Section, "Cosmetic Limit", 15, """
                Defines the maximum number of cosmetics that can be spawned for the player. Set the value to -1 to disable the limit
                """);
    }
}
