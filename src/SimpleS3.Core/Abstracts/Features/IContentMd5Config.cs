using System;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Abstracts.Features
{
    public interface IContentMd5Config : IHasContentMd5
    {
        Func<bool> ForceContentMd5 { get; }
    }
}