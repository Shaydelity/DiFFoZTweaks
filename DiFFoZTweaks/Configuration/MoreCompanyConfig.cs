using BepInEx.Configuration;

namespace DiFFoZTweaks.Configuration;
public class MoreCompanyConfig : ConfigSection
{
    public MoreCompanyConfig(ConfigFile config)
        : base(config, "Mods - MoreCompany", true)
    {
    }

    public ConfigEntryCheck<int> CosmeticLimit { get; private set; } = null!;

    protected override void Initialize(ConfigFile config)
    {
        CosmeticLimit = ConfigEntryCheck.Bind(config, Enabled, Section, "Cosmetic Limit", 15, """
                Defines the maximum number of cosmetics that can be spawned for the player. Set the value to -1 to disable the limit
                """);
    }
}
