using System;

namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasLastModified
    {
        DateTimeOffset? LastModified { get; }
    }
}