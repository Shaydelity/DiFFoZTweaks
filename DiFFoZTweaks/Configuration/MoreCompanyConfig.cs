using BepInEx.Configuration;

namespace DiFFoZTweaks.Configuration;
public class MoreCompanyConfig : ConfigSection
{
    public override string Section { get; protected set; } = "Mods - MoreCompany";

    public ConfigEntry<int> CosmeticLimit { get; private set; } = null!;

    public override void Initialize(ConfigFile config)
    {
        base.Initialize(config);
        Enabled.Value = true;

        CosmeticLimit = config.Bind(Section, "Cosmetic Limit", 15, """
                Defines the maximum number of cosmetics that can be spawned for the player. Set the value to -1 to disable the limit
                """);
    }
}
