namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Identity
{
    public S3Identity(string id, string? name)
    {
        Name = name;
        Id = id;
    }

    /// <summary>The display name of the identity</summary>
    public string? Name { get; }

    /// <summary>The unique identifier of the identity</summary>
    public string Id { get; }
}