using System.Collections.Generic;
using System.Text;

namespace Genbox.SimpleS3.Core.Internal.Helpers
{
    internal static class JsonHelper
    {
        public static string EncodeJson(IDictionary<string, string> dict)
        {
            //We build JSON here ourselves
            StringBuilder sb = new StringBuilder();

            int count = dict.Count;

            foreach (KeyValuePair<string, string> pair in dict)
            {
                sb.Append('"').Append(EscapeString(pair.Key)).Append("\":\"").Append(EscapeString(pair.Value)).Append('"');

                if (--count != 0)
                    sb.Append(',');
            }

            return sb.ToString();
        }

        private static string EscapeString(string input)
        {
            StringBuilder escaped = new StringBuilder(input.Length);

            //See http://json.org/
            foreach (char c in input)
            {
                if (c == '"' || c == '\\' || c == '/' || c == '\b' || c == '\f' || c == '\n' || c == '\r' || c == '\t')
                    escaped.Append('\\');

                escaped.Append(c);
            }

            return escaped.ToString();
        }
    }
}