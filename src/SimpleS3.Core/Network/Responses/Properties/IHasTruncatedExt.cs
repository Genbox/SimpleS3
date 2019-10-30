using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasTruncatedExt
    {
        /// <summary>
        /// Keys that begin with the indicated prefix.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Causes keys that contain the same string between the prefix and the first occurrence of the delimiter to be rolled up into a single result element in the CommonPrefixes collection. These rolled-up keys are not returned elsewhere in the response. Each rolled-up result counts as only one return against the MaxKeys value.
        /// </summary>
        string Delimiter { get; }

        /// <summary>
        /// All of the keys rolled up into a common prefix count as a single return when calculating the number of returns. A response can contain CommonPrefixes only if you specify a delimiter. CommonPrefixes contains all (if there are any) keys between Prefix and the next occurrence of the string specified by a delimiter. CommonPrefixes lists keys that act like subdirectories in the directory specified by Prefix. For example, if the prefix is notes/ and the delimiter is a slash (/) as in notes/summer/july, the common prefix is notes/summer/. All of the keys that roll up into a common prefix count as a single return when calculating the number of returns.
        /// </summary>
        IList<string> CommonPrefixes { get; }
    }
}