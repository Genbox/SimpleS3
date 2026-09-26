using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// The Multi-Object Delete operation enables you to delete multiple objects from a bucket using a single HTTP request. If you know the object keys that you want to delete,
/// then this operation provides a suitable alternative to sending individual delete requests (see DELETE Object), reducing per-request overhead.
/// </summary>
public sealed class DeleteObjectsRequest : BaseRequest, IHasRequestPayer, IHasBypassGovernanceRetention, IHasBucketName, IHasMfa, IContentMd5Config
{
    internal DeleteObjectsRequest() : base(HttpMethodType.POST)
    {
        Mfa = new MfaAuthenticationBuilder();
        Quiet = true;
        Objects = new List<S3DeleteInfo>();
    }

    /// <summary>Provides the <c>DeleteObjectsRequest</c> API.</summary>
    [SuppressMessage("Design", "CA1062", Justification = "Initialize consumes the required collection before construction completes; preserve its existing null-input failure behavior.")]
    public DeleteObjectsRequest(string bucketName, IEnumerable<S3DeleteInfo> resources) : this()
    {
        Initialize(bucketName, resources);
    }

    /// <summary>In quiet mode the response includes only keys where the delete operation encountered an error.</summary>
    public bool Quiet { get; set; }

    /// <summary>The list of objects</summary>
    public IList<S3DeleteInfo> Objects { get; }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>BypassGovernanceRetention</c> API.</summary>
    public bool? BypassGovernanceRetention { get; set; }

    /// <summary>Provides the <c>Mfa</c> API.</summary>
    public MfaAuthenticationBuilder Mfa { get; }

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    internal void Initialize(string bucketName, IEnumerable<S3DeleteInfo> resources)
    {
        BucketName = bucketName;

        foreach (S3DeleteInfo info in resources)
            Objects.Add(info);
    }

    internal override void Reset()
    {
        Objects.Clear();
        Mfa.Reset();
        Quiet = true;
        BypassGovernanceRetention = null;
        ContentMd5 = null;
        RequestPayer = Payer.Unknown;

        BucketName = null!;
        base.Reset();
    }
}