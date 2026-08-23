using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.PostMappers;

internal sealed class UploadPartCopyPostMapper : IPostMapper<UploadPartCopyRequest, UploadPartCopyResponse>
{
    public void PostMap(UploadPartCopyRequest request, UploadPartCopyResponse response)
    {
        if (request.PartNumber.HasValue)
            response.PartNumber = request.PartNumber.Value;
    }
}