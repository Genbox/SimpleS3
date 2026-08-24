// ReSharper disable InconsistentNaming
namespace Genbox.SimpleS3.Core.Abstracts.Enums;

/// <summary>Provides the <c>HttpMethodType</c> API.</summary>
public enum HttpMethodType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>GET</c> API.</summary>
    GET,
    /// <summary>Provides the <c>PUT</c> API.</summary>
    PUT,
    /// <summary>Provides the <c>HEAD</c> API.</summary>
    HEAD,
    /// <summary>Provides the <c>DELETE</c> API.</summary>
    DELETE,
    /// <summary>Provides the <c>POST</c> API.</summary>
    POST
}