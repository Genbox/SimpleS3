namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

/// <summary>Provides the <c>IHasExpectedBucketOwner</c> API.</summary>
public interface IHasExpectedBucketOwner
{
    /// <summary>The account id of the expected bucket owner. If the bucket is owned by a different account, the request will fail with an HTTP 403 (Access Denied) error.</summary>
    string? ExpectedBucketOwner { get; set; }
}