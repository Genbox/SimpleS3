using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers;

public interface IResponseWrapper
{
    bool IsSupported(IResponse response);

    ContentStream Wrap(ContentStream input, IResponse response);
}