using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Mono.Cecil;

namespace DiFFoZTweaks.Patcher;

internal class DiFFoZTweaksPatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = [];

    public static DiFFoZTweaksPatcher Instance { get; private set; } = null!;

    private Harmony m_Harmony = null!;

    private void Initialize()
    {
        m_Harmony = new(nameof(DiFFoZTweaksPatcher));
        m_Harmony.PatchAll(typeof(DiFFoZTweaksPatcher).Assembly);


    }

    public static void Finish()
    {
        Instance = new();
        Instance.Initialize();
    }

    public static void Patch(AssemblyDefinition _) { }
}
