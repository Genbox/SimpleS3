using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

/// <summary>Provides the <c>IProfile</c> API.</summary>
[PublicAPI]
public interface IProfile
{
    /// <summary>Provides the <c>Name</c> API.</summary>
    string Name { get; set; }

    /// <summary>Provides the <c>KeyId</c> API.</summary>
    string KeyId { get; set; }

    /// <summary>Provides the <c>AccessKey</c> API.</summary>
    byte[] AccessKey { get; set; }

    /// <summary>Provides the <c>RegionCode</c> API.</summary>
    string RegionCode { get; set; }

    /// <summary>Provides the <c>CreatedOn</c> API.</summary>
    DateTimeOffset CreatedOn { get; set; }

    /// <summary>Provides the <c>ProfileVersion</c> API.</summary>
    int ProfileVersion { get; set; }

    /// <summary>Provides the <c>Tags</c> API.</summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "The established public contract requires a setter so profile deserializers can initialize tags.")]
    IDictionary<string, string>? Tags { get; set; }

    /// <summary>Provides the <c>GetTag</c> API.</summary>
    string? GetTag(string key);
}