using System;
using System.Collections.Generic;
using System.Reflection;
using DiFFoZTweaks.Harmony;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace DiFFoZTweaks.MoreCompany;
[HarmonyPatch]
internal static class Patch_CosmeticApplication
{
    private static readonly MethodBase? s_ApplyCosmetic;

    static Patch_CosmeticApplication()
    {
        if (!Dependencies.TryGetMod(Dependencies.MoreCompany, out var plugin))
        {
            return;
        }

        var cosmeticApplicationType = plugin.Instance.GetType().Assembly.GetType("MoreCompany.Cosmetics.CosmeticApplication");
        if (cosmeticApplicationType == null)
        {
            return;
        }

        s_ApplyCosmetic = cosmeticApplicationType.GetMethod("ApplyCosmetic", AccessTools.all);
    }

    [HarmonyCleanup]
    public static Exception? Cleanup(Exception exception)
    {
        return HarmonyExceptionHandler.ReportException(exception);
    }

    [HarmonyPrepare]
    internal static bool ShouldPatch()
    {
        return s_ApplyCosmetic != null;
    }

    [HarmonyTargetMethod]
    internal static MethodBase GetTargetMethod()
    {
        return s_ApplyCosmetic!;
    }

    [HarmonyPrefix]
    public static bool CheckCosmeticLimit(MonoBehaviour __instance, List<string> ___spawnedCosmeticsIds, out bool __result)
    {
        if (!DiFFoZTweaksPlugin.Instance.Config.MoreCompany.CosmeticLimit.TryGetValue(out var limitCount)
            || limitCount < 0)
        {
            __result = false;
            return true;
        }

        var player = __instance.GetComponentInParent<PlayerControllerB>();
        if (player == null)
        {
            __result = false;
            return true;
        }

        if (GameNetworkManager.Instance != null
            && GameNetworkManager.Instance.localPlayerController == player)
        {
            __result = false;
            return true;
        }

        // +1 to include this cosmetic
        if (___spawnedCosmeticsIds.Count + 1 > limitCount)
        {
            DiFFoZTweaksPlugin.Instance.Logger.LogMessage($"Ignoring cosmetic spawn for {player.playerUsername} due to reaching the limit of {limitCount} cosmetics");

            __result = false;
            return false;
        }

        __result = false;
        return true;
    }
}
