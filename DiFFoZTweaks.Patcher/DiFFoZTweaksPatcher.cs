using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil;

namespace DiFFoZTweaks.Patcher;

internal class DiFFoZTweaksPatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = [];

    public static DiFFoZTweaksPatcher Instance { get; private set; } = null!;

    public Harmony Harmony { get; private set; } = null!;
    public ManualLogSource Logger { get; private set; } = null!;

    private void Initialize()
    {
        Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(DiFFoZTweaksPatcher));

        Harmony = new(nameof(DiFFoZTweaksPatcher));
        Harmony.PatchAll(typeof(DiFFoZTweaksPatcher).Assembly);
    }

    public static void Finish()
    {
        Instance = new();
        Instance.Initialize();
    }

    public static void Patch(AssemblyDefinition _) { }
}
