using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.PostMappers
{
    internal class UploadPartPostMapper : IPostMapper<UploadPartRequest, UploadPartResponse>
    {
        public void PostMap(Config config, UploadPartRequest request, UploadPartResponse response)
        {
            if (request.PartNumber.HasValue)
                response.PartNumber = request.PartNumber.Value;
        }
    }
}