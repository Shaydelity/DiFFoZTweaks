using System;
using BepInEx.Configuration;

namespace DiFFoZTweaks.Patcher.Configuration;
public class LazyConfigEntryCheck<T, U> : ConfigEntryCheck<T>
{
    private readonly Func<T, U> m_Parser;

    public LazyConfigEntryCheck(ConfigEntry<bool> enabledEntry, ConfigEntry<T> configEntry, Func<T, U> parser) : base(enabledEntry, configEntry)
    {
        m_Parser = parser;

        Value = new Lazy<U>(ParseValue, true);
    }

    public Lazy<U> Value { get; private set; }

    private U ParseValue()
    {
        return m_Parser(m_ConfigEntry.Value);
    }
}
