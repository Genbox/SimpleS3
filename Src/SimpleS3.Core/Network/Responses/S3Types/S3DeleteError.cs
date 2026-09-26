using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3DeleteError</c> API.</summary>
public class S3DeleteError : IHasVersionId, IHasObjectKey
{
    /// <summary>Creates an error returned for an individual object in a bulk delete.</summary>
    public S3DeleteError(string? objectKey, ErrorCode code, string? message, string? versionId)
    {
        Code = code;
        Message = message;
        ObjectKey = objectKey;
        VersionId = versionId;
    }

    /// <summary>Provides the <c>Code</c> API.</summary>
    public ErrorCode Code { get; }

    /// <summary>Provides the <c>Message</c> API.</summary>
    public string? Message { get; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string? ObjectKey { get; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; }

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => $"Error on {ObjectKey}: {Message}";
}