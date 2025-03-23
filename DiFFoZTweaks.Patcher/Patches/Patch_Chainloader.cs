using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using Mono.Cecil;
using static System.Reflection.Emit.OpCodes;

namespace DiFFoZTweaks.Patcher.Patches;
[HarmonyPatch(typeof(Chainloader))]
internal static class Patch_Chainloader
{
    [HarmonyPatch(nameof(Chainloader.Initialize))]
    [HarmonyPostfix]
    public static void PatchAfterUnity()
    {
        DiFFoZTweaksPatcher.Instance.Harmony.CreateProcessor(AccessTools.Method(typeof(Chainloader), nameof(Chainloader.Start)))
            .AddTranspiler(SymbolExtensions.GetMethodInfo(() => PatchPluginTranspiler(null!)))
            .Patch();
    }

    public static IEnumerable<CodeInstruction> PatchPluginTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        // if (!dictionary2.TryGetValue(pluginInfo5.Location, out assembly))
        matcher.MatchForward(false, [
            new(IsLdLoc),
            new(IsLdLoc, name: "pluginInfo"),
            new(Callvirt),
            new(IsLdLoc, name: "assembly"),
            new(Callvirt),
            new(Brtrue, name: "assemblyLoadedLabel")
            ])
            .GetNamedMatchOperand("pluginInfo", out LocalBuilder info)
            .GetNamedMatchOperand("assembly", out LocalBuilder assembly)
            .GetNamedMatchOperand("assemblyLoadedLabel", out Label label)
            .Insert([
                new(Ldloc, info),
                new(Ldloca, assembly),
                new(Call, SymbolExtensions.GetMethodInfo((Assembly b) => PatchPlugin(null!, out b!))),
                new(Brtrue, label)
                ]);


        return matcher.Instructions();
    }

    private static bool PatchPlugin(PluginInfo info, out Assembly? assembly)
    {
        assembly = null;
        return false;

        using var definition = AssemblyDefinition.ReadAssembly(info.Location, TypeLoader.ReaderParameters);
        var method = definition.MainModule
            .GetType("WesleyMoonScripts.Components", "ItemShop")
            .Methods.FirstOrDefault(m => m.Name == "Start");

        if (method == null)
        {
            return false;
        }

        method.Name = "OnNetworkSpawn";
        method.Attributes |= Mono.Cecil.MethodAttributes.CheckAccessOnOverride | Mono.Cecil.MethodAttributes.Virtual;

        Type.GetType("BepInEx.Preloader.Patching.AssemblyPatcher, BepInEx.Preloader")
            .GetMethod("Load", AccessTools.all)
            .Invoke(null, [definition, ""]);

        assembly = Type.GetType(method.DeclaringType.FullName + "," + method.Module.Assembly.FullName).Assembly;
        DiFFoZTweaksPatcher.Instance.Logger.LogMessage("Patched " + info.Metadata.GUID);

        return true;
    }

    private static bool IsLdLoc(CodeInstruction instruction)
    {
        return instruction.IsLdloc();
    }

    private static CodeMatcher GetNamedMatchOperand<T>(this CodeMatcher matcher, string name, out T result)
    {
        result = (T)matcher.NamedMatch(name).operand;
        return matcher;
    }
}
