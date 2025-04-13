using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DiFFoZTweaks.Extensions;
using DiFFoZTweaks.Utilities;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;

namespace DiFFoZTweaks.Patches;
[HarmonyPatch(typeof(AudioReverbTrigger))]
internal static class Patch_AudioReverbTrigger
{
    [HarmonyCleanup]
    public static Exception? Cleanup(Exception exception)
    {
        return HarmonyExceptionHandler.ReportException(exception);
    }

    [HarmonyPatch("Start")] // added by preloader
    [HarmonyPostfix]
    public static void ValidateAudioSources(AudioReverbTrigger __instance)
    {
        if (__instance.audioChanges == null)
        {
            DiFFoZTweaksPlugin.Instance.Logger.LogError($"AudioChanges is null\nAudioReverbTrigger scene path: {__instance.transform.GetScenePath()}");
            return;
        }

        foreach (var change in __instance.audioChanges)
        {
            if (change.audio != null)
            {
                continue;
            }

            DiFFoZTweaksPlugin.Instance.Logger.LogError($"""
                AudioReverbTrigger contains null AudioSource. Please report it to the moon developer
                AudioReverbTrigger scene path: {__instance.transform.GetScenePath()}
                """);
            return;
        }
    }

    [HarmonyPatch(nameof(AudioReverbTrigger.ChangeAudioReverbForPlayer))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CheckAudioSourceExists(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        //         AudioSource audio = this.audioChanges[i].audio;
        /*
			IL_02AB: ldarg.0
			IL_02AC: ldfld     class switchToAudio[] AudioReverbTrigger::audioChanges
			IL_02B1: ldloc.3
			IL_02B2: ldelem.ref
			IL_02B3: ldfld     class [UnityEngine.AudioModule]UnityEngine.AudioSource switchToAudio::audio
			IL_02B8: stloc.s   V_4
        */

        var posToInject = matcher.MatchForward(true,
            [
            new(i => i.opcode == Ldfld && i.operand is FieldInfo fi && fi.FieldType == typeof(AudioSource)),
            new(i => i.opcode == Stloc_S && i.operand is LocalBuilder lb && lb.LocalType == typeof(AudioSource), name: "audioSource")
            ])
            .ThrowIfInvalid("Failed to find stloc of AudioSource")
            .Advance(1)
            .Pos;

        var audioSourceLoc = matcher.NamedMatch("audioSource")
            .operand;

        // find the opcode "br" to move next index
        var label = matcher.SearchForward(i => i.opcode == Br)
            .ThrowIfInvalid("Failed to find for move next label")
            .Operand;

        // if (audio == null) continue;
        matcher.Start()
            .Advance(posToInject)
            .Insert([
                new(Ldloc, audioSourceLoc),
                new(Ldnull),
                new(Call, typeof(Object).GetMethod("op_Equality", AccessTools.all)),
                new(Brtrue, label)
                ]);

        return matcher.Instructions();
    }
}
