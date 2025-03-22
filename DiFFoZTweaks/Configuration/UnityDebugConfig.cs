using BepInEx.Configuration;
using UnityEngine;

namespace DiFFoZTweaks.Configuration;

public class UnityDebugConfig : ConfigSection
{
    public UnityDebugConfig(ConfigFile config) : base(config, "Unity - Debug", true, "Enable unity debug settings")
    {
    }

    public ConfigEntryCheck<bool>? EnableStacktraces { get; private set; }
    public ConfigEntryCheck<bool>? ApplyThreadSafetyPath { get; private set; }

    protected override void Initialize(ConfigFile config)
    {
        if (!Debug.isDebugBuild)
        {
            return;
        }

        EnableStacktraces = Bind("Enable stacktraces", false, "Enables stacktraces via Application.SetStackTraceLogType");
        ApplyThreadSafetyPath = Bind("Enable thread safety check", false, "Enables thread safety check");
    }
}