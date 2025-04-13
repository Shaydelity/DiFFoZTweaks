using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DiFFoZTweaks.Patcher;

internal class DiFFoZTweaksPatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp.dll"];

    public static DiFFoZTweaksPatcher Instance { get; private set; } = null!;

    public Harmony Harmony { get; private set; } = null!;
    public ManualLogSource Logger { get; private set; } = null!;

    public static void Initialize()
    {
        Instance = new();
        Instance.InternalInitialize();
    }

    private void InternalInitialize()
    {
        Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(DiFFoZTweaksPatcher));
    }

    public static void Finish()
    {
        var instance = Instance;

        instance.Harmony = new(nameof(DiFFoZTweaksPatcher));
        instance.Harmony.PatchAll(typeof(DiFFoZTweaksPatcher).Assembly);
    }

    public static void Patch(AssemblyDefinition assembly)
    {
        var audioReverbTriggerType = assembly.MainModule.GetType("", "AudioReverbTrigger");
        if (audioReverbTriggerType.Methods.Any(m => m.Name == "Start"))
        {
            return;
        }

        var startMethod = new MethodDefinition("Start",
            MethodAttributes.Private | MethodAttributes.HideBySig,
            assembly.MainModule.ImportReference(typeof(void)));

        var il = startMethod.Body.GetILProcessor();
        for (var i = 0; i < 32; i++)
        {
            il.Append(Instruction.Create(OpCodes.Nop));
        }
        il.Append(Instruction.Create(OpCodes.Ret));

        audioReverbTriggerType.Methods.Add(startMethod);
    }
}
