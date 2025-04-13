using System.Text;
using UnityEngine;

namespace DiFFoZTweaks.Extensions;
internal static class TransformExtension
{
    public static string GetScenePath(this Transform transform)
    {
        var sb = new StringBuilder();
        sb.Append('/').Append(transform.name);

        while ((transform = transform.parent) != null)
        {
            sb.Insert(0, transform.name)
                .Insert(0, '/');
        }

        return sb.ToString();
    }
}
