namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

public interface IHasETag
{
    /// <summary>The entity tag is a hash of the object. The ETag reflects changes only to the contents of an object, not its
    /// metadata.</summary>
    string? ETag { get; }
}