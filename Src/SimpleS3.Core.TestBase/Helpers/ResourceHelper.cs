using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Genbox.SimpleS3.Core.TestBase.Helpers;

[SuppressMessage("Design", "CA1062", Justification = "Test resource helpers use non-null assemblies and filters supplied by their fixtures and preserve deferred enumeration.")]
public static class ResourceHelper
{
    public static IEnumerable<(string name, string content)> GetResources(Assembly assembly, Regex filter)
    {
        foreach (string resourceName in assembly.GetManifestResourceNames())
        {
            if (!filter.IsMatch(resourceName))
                continue;

            yield return (resourceName, GetResource(assembly, resourceName));
        }
    }

    public static string GetResource(Assembly assembly, string name)
    {
        using MemoryStream ms = new MemoryStream();
        using Stream? s = assembly.GetManifestResourceStream(name);
        if (s == null)
            return string.Empty;

        s.CopyTo(ms);

        return Encoding.UTF8.GetString(ms.ToArray());
    }
}