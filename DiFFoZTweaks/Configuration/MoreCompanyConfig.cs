using BepInEx.Configuration;

namespace DiFFoZTweaks.Configuration;
public class MoreCompanyConfig : ConfigSection
{
    public override string Section { get; protected set; } = "Mods - MoreCompany";

    public ConfigEntryCheck<int> CosmeticLimit { get; private set; } = null!;

    public override void Initialize(ConfigFile config)
    {
        InitializeEnabled(config, true);

        CosmeticLimit = ConfigEntryCheck.Bind(config, Enabled, Section, "Cosmetic Limit", 15, """
                Defines the maximum number of cosmetics that can be spawned for the player. Set the value to -1 to disable the limit
                """);
    }
}
