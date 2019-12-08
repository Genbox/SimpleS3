using System;

namespace Genbox.SimpleS3.Core.Abstracts.Features
{
    public interface IContentMd5Config
    {
        Func<bool> ForceContentMd5 { get; }
    }
}