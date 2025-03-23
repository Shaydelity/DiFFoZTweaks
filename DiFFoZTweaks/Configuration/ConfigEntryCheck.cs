using System;
using BepInEx.Configuration;

namespace DiFFoZTweaks.Configuration;

public static class ConfigEntryCheck
{
    public static ConfigEntryCheck<T> Bind<T>(this ConfigFile config, ConfigEntry<bool> enabledEntry, string section, string key, T defaultValue, string description)
    {
        var entry = config.Bind(section, key, defaultValue, description);
        return new ConfigEntryCheck<T>(enabledEntry, entry);
    }
}

public class ConfigEntryCheck<T>
{
    private readonly ConfigEntry<bool> m_EnabledEntry;
    private readonly ConfigEntry<T> m_ConfigEntry;

    public ConfigEntryCheck(ConfigEntry<bool> enabledEntry, ConfigEntry<T> configEntry)
    {
        m_EnabledEntry = enabledEntry;
        m_ConfigEntry = configEntry;
    }

    public bool TryGetValue(out T? value)
    {
        if (!m_EnabledEntry.Value)
        {
            value = default;
            return false;
        }

        value = m_ConfigEntry.Value;
        return true;
    }

    public void SubscribeAndInvoke(Action<ConfigEntryCheck<T>> action)
    {
        void EventHandler(object _, System.EventArgs __)
        {
            // capture this sender instead
            action(this);
        }

        m_EnabledEntry.SettingChanged += EventHandler;
        m_ConfigEntry.SettingChanged += EventHandler;

        action(this);
    }
}
