namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasUploadId</c> API.</summary>
public interface IHasUploadId
{
    /// <summary>Provides the <c>UploadId</c> API.</summary>
    string UploadId { get; }
}