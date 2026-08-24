namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Identity</c> API.</summary>
public class S3Identity(string id, string? name)
{
    /// <summary>The display name of the identity</summary>
    public string? Name { get; } = name;

    /// <summary>The unique identifier of the identity</summary>
    public string Id { get; } = id;
}