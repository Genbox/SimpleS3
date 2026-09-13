using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class FilterHelper
{
    public static void Validate(S3Filter filter)
    {
        int predicates = (filter.Prefix != null ? 1 : 0) + (filter.Tag.HasValue ? 1 : 0) + (filter.AndConditions != null ? 1 : 0)
                         + (filter.ObjectSizeGreaterThan.HasValue ? 1 : 0) + (filter.ObjectSizeLessThan.HasValue ? 1 : 0);
        if (predicates > 1)
            throw new ArgumentException("Lifecycle filters require multiple predicates to be combined inside AndConditions.", nameof(filter));
    }
}