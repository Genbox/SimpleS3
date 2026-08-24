using Genbox.HttpBuilders.Abstracts;

namespace Genbox.SimpleS3.Core.Builders;

/// <summary>Provides the <c>MfaAuthenticationBuilder</c> API.</summary>
public class MfaAuthenticationBuilder : IHttpHeaderBuilder
{
    private string? _serialNumber;
    private string? _value;

    /// <summary>Provides the <c>HeaderName</c> API.</summary>
    public string? HeaderName => null;

    /// <summary>Provides the <c>Build</c> API.</summary>
    public string? Build()
    {
        if (!HasData())
            return null;

        return $"{_serialNumber} {_value}";
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public void Reset()
    {
        _serialNumber = null;
        _value = null;
    }

    /// <summary>Provides the <c>HasData</c> API.</summary>
    public bool HasData() => _serialNumber != null;

    /// <summary>Provides the <c>SetAuth</c> API.</summary>
    public void SetAuth(string serialNumber, string value)
    {
        _serialNumber = serialNumber;
        _value = value;
    }
}