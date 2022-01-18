using System;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

public interface IHasAbort
{
    /// <summary>
    /// If the bucket has a lifecycle rule configured with an action to abort incomplete multipart uploads and the prefix in the lifecycle rule
    /// matches the object name in the request, the response includes this header. The header indicates when the initiated multipart upload becomes eligible
    /// for an abort operation.
    /// </summary>
    DateTimeOffset? AbortsOn { get; }

    /// <summary>
    /// This header is returned along with the x-amz-abort-date header. It identifies the applicable lifecycle configuration rule that defines the
    /// action to abort incomplete multipart uploads.
    /// </summary>
    string? AbortRuleId { get; }
}