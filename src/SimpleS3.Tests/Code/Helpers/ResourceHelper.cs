using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Genbox.SimpleS3.Tests.Code.Helpers
{
    public static class ResourceHelper
    {
        [ThreadStatic]
        private static Assembly _cachedAssembly;

        private static Assembly _assembly => _cachedAssembly ??= typeof(ResourceHelper).Assembly;

        public static IEnumerable<(string name, string content)> GetResources(string filter)
        {
            Regex regex = new Regex(filter, RegexOptions.Compiled);

            foreach (string resourceName in _assembly.GetManifestResourceNames())
            {
                if (!regex.IsMatch(resourceName))
                    continue;

                yield return (resourceName, GetResource(resourceName));
            }
        }

        public static string GetResource(string name)
        {
            using (MemoryStream ms = new MemoryStream())
            using (Stream s = _assembly.GetManifestResourceStream(name))
            {
                s.CopyTo(ms);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}