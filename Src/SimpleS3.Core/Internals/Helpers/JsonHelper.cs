using System.Text;
using Genbox.SimpleS3.Core.Common.Pools;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class JsonHelper
{
    public static string EncodeJson(IDictionary<string, string> dict)
    {
        //We build JSON here ourselves
        StringBuilder sb = StringBuilderPool.Shared.Rent(dict.Count * 15);

        int count = dict.Count;

        foreach (KeyValuePair<string, string> pair in dict)
        {
            sb.Append('"');
            EscapeString(sb, pair.Key);
            sb.Append("\":\"");
            EscapeString(sb, pair.Value);
            sb.Append('"');

            if (--count != 0)
                sb.Append(',');
        }

        return StringBuilderPool.Shared.ReturnString(sb);
    }

    private static void EscapeString(StringBuilder sb, string input)
    {
        //See http://json.org/
        foreach (char c in input)
        {
            if (c == '"' || c == '\\' || c == '/' || c == '\b' || c == '\f' || c == '\n' || c == '\r' || c == '\t')
                sb.Append('\\');

            sb.Append(c);
        }
    }
}