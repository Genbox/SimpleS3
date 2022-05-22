using System.Text;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Cli.Core.Helpers;

public static class PathHelper
{
    public static string Combine(char orgChar, char replaceChar, params string?[] components)
    {
        Validator.RequireValueAndItemsNotNull(components);

        StringBuilder sb = StringBuilderPool.Shared.Rent();

        for (int i = 0; i < components.Length; i++)
        {
            string component = components[i]!;

            if (component.Length == 0)
                continue;

            component = component.Replace(orgChar, replaceChar);

            sb.Append(component.Trim(replaceChar));

            if (i < components.Length - 1)
                sb.Append(replaceChar);
            else
            {
                //Preserve slash at the end of the path if it was originally given
                if (component.EndsWith(replaceChar))
                    sb.Append(replaceChar);
            }
        }

        return StringBuilderPool.Shared.ReturnString(sb);
    }
}