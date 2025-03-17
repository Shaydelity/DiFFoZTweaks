using System;

namespace DiFFoZTweaks.Utilities;
internal static class HarmonyExceptionHandler
{
    public static Exception? ReportException(Exception? exception)
    {
        if (exception != null)
        {
            DiFFoZTweaksPlugin.Instance.Logger.LogWarning(exception);
            // stacktrace needed to find class that fails to patch
            DiFFoZTweaksPlugin.Instance.Logger.LogWarning(Environment.StackTrace);
        }

        return null;
    }
}
