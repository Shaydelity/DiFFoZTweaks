using System.Collections.Generic;
using HarmonyLib;
using SoftMasking;

namespace DiFFoZTweaks.Patches;

[HarmonyPatch(typeof(MaterialReplacer))]
internal static class Patch_MaterialReplacer
{
    [HarmonyPatch(nameof(MaterialReplacer.globalReplacers), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool NoGlobalReplacers(out IEnumerable<IMaterialReplacer> __result)
    {
        __result = [];
        return false;
    }
}
