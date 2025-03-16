using BepInEx.Bootstrap;

namespace DiFFoZTweaks;
internal static class Dependencies
{
    public const string MoreCompany = "me.swipez.melonloader.morecompany";

    public static bool TryGetMod(string id, out BepInEx.PluginInfo pluginInfo)
    {
        return Chainloader.PluginInfos.TryGetValue(id, out pluginInfo);
    }
}
