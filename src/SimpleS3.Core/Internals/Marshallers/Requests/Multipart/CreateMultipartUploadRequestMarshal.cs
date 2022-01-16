using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart
{
    internal class CreateMultipartUploadRequestMarshal : IRequestMarshal<CreateMultipartUploadRequest>
    {
        public Stream? MarshalRequest(CreateMultipartUploadRequest request, Config config)
        {
            //This is required for multipart uploads
            request.SetQueryParameter(AmzParameters.Uploads, string.Empty);
            return null;
        }
    }
}