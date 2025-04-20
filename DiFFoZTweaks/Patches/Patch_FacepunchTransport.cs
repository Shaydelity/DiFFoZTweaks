using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using DiFFoZTweaks.Utilities;
using HarmonyLib;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;

namespace DiFFoZTweaks.Patches;
[HarmonyPatch(typeof(FacepunchTransport))]
internal static class Patch_FacepunchTransport
{
    private static readonly ManualLogSource s_Logger = BepInEx.Logging.Logger.CreateLogSource("FacepunchTransport");

    [HarmonyCleanup]
    public static Exception? Cleanup(Exception exception)
    {
        return HarmonyExceptionHandler.ReportException(exception);
    }

    [HarmonyPatch(nameof(FacepunchTransport.Send))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> LogSendMessageResult(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(true,
            [
            new (i => i.opcode == OpCodes.Call
                && i.operand is MethodInfo mi
                && mi.Name == "SendMessage"),
                new(OpCodes.Pop)
            ])
            .Repeat(m =>
            {
                m.Set(OpCodes.Call, ((Delegate)LogSendResult).Method);
            });

        return matcher.Instructions();
    }

    [HarmonyPatch("NetworkDeliveryToSendType")] // oh wow, it's not publicized
    [HarmonyPrefix]
    public static bool NetworkDeliveryToSendTypeNoNagle(NetworkDelivery delivery, out SendType __result)
    {
        __result = delivery switch
        {
            NetworkDelivery.UnreliableSequenced => SendType.Unreliable | SendType.NoNagle,
            NetworkDelivery.Reliable or NetworkDelivery.ReliableFragmentedSequenced => SendType.Reliable,
            NetworkDelivery.ReliableSequenced => SendType.Reliable | SendType.NoNagle,
            _ => SendType.Unreliable,
        };
        return false;
    }

    private static void LogSendResult(int result)
    {
        var eResult = (Result)result;
        if (eResult is not Result.OK)
        {
            s_Logger.LogWarning("Send message returned not ok status: " + eResult.ToString());
        }
    }
}
