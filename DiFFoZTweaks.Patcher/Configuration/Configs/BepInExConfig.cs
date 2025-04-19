using System;
using BepInEx.Configuration;

namespace DiFFoZTweaks.Patcher.Configuration.Configs;
public class BepInExConfig : ConfigSection
{
    public BepInExConfig(ConfigFile config)
        : base(config, "General - BepInEx", false, "Should patch BepInEx")
    {
    }

    public LazyConfigEntryCheck<string, string[]> PluginsToNotLoad { get; private set; } = null!;

    protected override void Initialize(ConfigFile config)
    {
        PluginsToNotLoad = config.BindLazy(Enabled, ParseAsArray, Section, "Plugins to not load", "", """
                A list of GUID plugins that should be not loaded. Separator: ','.
                Note: BepInEx caches plugins metadata, so if you add or remove plugin here you should delete cache folder in the profile path.

                Example: TestAccount666.ShipWindows, LethalPerformance
                """);
    }

    private static string[] ParseAsArray(string input)
    {
        return input.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
}
