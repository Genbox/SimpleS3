using System;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

public interface IHasLastModified
{
    DateTimeOffset? LastModified { get; }
}