using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Genbox.SimpleS3.Core.TestBase.Helpers
{
    public static class ResourceHelper
    {
        public static IEnumerable<(string name, string content)> GetResources(Assembly assembly, string filter)
        {
            Regex regex = new Regex(filter, RegexOptions.Compiled);

            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (!regex.IsMatch(resourceName))
                    continue;

                yield return (resourceName, GetResource(assembly, resourceName));
            }
        }

        public static string GetResource(Assembly assembly, string name)
        {
            using (MemoryStream ms = new MemoryStream())
            using (Stream? s = assembly.GetManifestResourceStream(name))
            {
                if (s == null)
                    return string.Empty;

                s.CopyTo(ms);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}