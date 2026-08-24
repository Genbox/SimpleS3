using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Network.Responses;

/// <summary>Provides the <c>BaseResponse</c> API.</summary>
public abstract class BaseResponse : IResponse
{
    /// <summary>Provides the <c>StatusCode</c> API.</summary>
    public int StatusCode { get; set; }

    /// <summary>Provides the <c>Error</c> API.</summary>
    public IError? Error { get; set; }

    /// <summary>Provides the <c>ContentLength</c> API.</summary>
    public long ContentLength { get; set; }

    /// <summary>Provides the <c>ConnectionClosed</c> API.</summary>
    public bool ConnectionClosed { get; set; }

    /// <summary>Provides the <c>Date</c> API.</summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>Provides the <c>Server</c> API.</summary>
    public string? Server { get; set; }

    /// <summary>Provides the <c>ResponseId</c> API.</summary>
    public string? ResponseId { get; set; }

    /// <summary>Provides the <c>RequestId</c> API.</summary>
    public string? RequestId { get; set; }

    /// <summary>Provides the <c>IsSuccess</c> API.</summary>
    public bool IsSuccess { get; set; }
}