using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class FilterHelper
{
    public static void Validate(S3Filter filter, string configuration, bool supportsSizes)
    {
        S3AndCondition? and = filter.AndConditions;
        if (!supportsSizes && (filter.ObjectSizeGreaterThan.HasValue || filter.ObjectSizeLessThan.HasValue || and?.ObjectSizeGreaterThan != null || and?.ObjectSizeLessThan != null))
            throw new ArgumentException($"{configuration} filters do not support object-size predicates.", nameof(filter));
        int predicates = (filter.Prefix != null ? 1 : 0) + (filter.Tag.HasValue ? 1 : 0) + (and != null ? 1 : 0)
                         + (filter.ObjectSizeGreaterThan.HasValue ? 1 : 0) + (filter.ObjectSizeLessThan.HasValue ? 1 : 0);
        if (predicates > 1)
            throw new ArgumentException($"{configuration} filters require multiple predicates to be combined inside AndConditions.", nameof(filter));
    }
}