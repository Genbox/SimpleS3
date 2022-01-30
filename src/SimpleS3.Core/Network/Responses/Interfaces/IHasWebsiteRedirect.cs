namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

public interface IHasWebsiteRedirect
{
    /// <summary>f the bucket is configured as a website, redirects requests for this object to another object in the same
    /// bucket or to an external URL. Amazon S3 stores the value of this header in the object metadata.</summary>
    string? WebsiteRedirectLocation { get; }
}