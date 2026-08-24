namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>IScopeBuilder</c> API.</summary>
public interface IScopeBuilder
{
    /// <summary>Provides the <c>CreateScope</c> API.</summary>
    string CreateScope(string service, DateTimeOffset date);
}