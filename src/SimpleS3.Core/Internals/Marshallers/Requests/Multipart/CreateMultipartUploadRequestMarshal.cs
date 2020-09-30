using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart
{
    [UsedImplicitly]
    internal class CreateMultipartUploadRequestMarshal : IRequestMarshal<CreateMultipartUploadRequest>
    {
        public Stream? MarshalRequest(CreateMultipartUploadRequest request, IConfig config)
        {
            //This is required for multipart uploads
            request.SetQueryParameter(AmzParameters.Uploads, string.Empty);
            return null;
        }
    }
}