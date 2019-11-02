using System;
using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Internal.Helpers
{
    internal static class MetadataHelper
    {
        public static IDictionary<string, string> ParseMetadata(IDictionary<string, string> headers)
        {
            string _metadataHeader = "x-amz-meta-";

            IDictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> item in headers)
            {
                if (!item.Key.StartsWith(_metadataHeader, StringComparison.OrdinalIgnoreCase))
                    continue;

                //If we crash here, it is because AWS sent us an invalid header.
                metadata[item.Key.Substring(_metadataHeader.Length)] = item.Value;
            }

            return metadata;
        }
    }
}
