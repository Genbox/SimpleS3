namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

public interface IScopeBuilder
{
    string CreateScope(string service, DateTimeOffset date);
}