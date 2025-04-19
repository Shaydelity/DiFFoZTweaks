using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiFFoZTweaks.Patcher.Configuration;
using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DiFFoZTweaks.Patcher;

public class DiFFoZTweaksPatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp.dll"];

    public static DiFFoZTweaksPatcher Instance { get; private set; } = null!;

    private ConfigFile m_ConfigFile = null!;

    public Harmony Harmony { get; private set; } = null!;
    public ManualLogSource Logger { get; private set; } = null!;
    public ConfigManager Config { get; private set; } = null!;

    public static void Initialize()
    {
        Instance = new();
        Instance.InternalInitialize();
    }

    private void InternalInitialize()
    {
        Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(DiFFoZTweaksPatcher));

        var configPath = Path.Combine(Paths.ConfigPath, "DiFFoZTweaks.cfg");

        m_ConfigFile = new ConfigFile(configPath, true);
        Config = new(m_ConfigFile);
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
