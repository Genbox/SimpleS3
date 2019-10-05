using System;

namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface IScopeBuilder
    {
        string CreateScope(string service, DateTimeOffset date);
    }
}