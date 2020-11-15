using System.IO;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Cli.Core.Helpers
{
    public static class RemotePathHelper
    {
        public static string Combine(params string?[] components)
        {
            Validator.RequireNotNullOrEmpty(components, nameof(components));
            Validator.RequireThat(components.Any(x => x != null), nameof(components));

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < components.Length; i++)
            {
                string component = components[i]!;

                if (component.Length == 0)
                    continue;

                sb.Append(component.Trim('/'));

                if (i < components.Length - 1)
                    sb.Append('/');
            }

            return sb.ToString();
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}