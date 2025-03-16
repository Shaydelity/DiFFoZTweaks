using System.Runtime.InteropServices;
using HarmonyLib;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DiFFoZTweaks.Patches;
[HarmonyPatch(typeof(PreInitSceneScript))]
internal static class Patch_PreInitSceneScript
{
    [HarmonyPatch(nameof(PreInitSceneScript.SetLaunchPanelsEnabled))]
    [HarmonyPostfix]
    internal static void FlashTaskbar()
    {
        if (!DiFFoZTweaksPlugin.Instance.Config.FlashTaskbar.Value)
        {
            return;
        }

        var hwnd = PInvoke.GetActiveWindow();
        if (hwnd == null)
        {
            return;
        }

        var flashInfo = new FLASHWINFO
        {
            cbSize = (uint)Marshal.SizeOf<FLASHWINFO>(),
            hwnd = hwnd,
            dwFlags = FLASHWINFO_FLAGS.FLASHW_ALL | FLASHWINFO_FLAGS.FLASHW_TIMERNOFG,
            uCount = uint.MaxValue,
            dwTimeout = 0
        };

        PInvoke.FlashWindowEx(in flashInfo);
    }
}
