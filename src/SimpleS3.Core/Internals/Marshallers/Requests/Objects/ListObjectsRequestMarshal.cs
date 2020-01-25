using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    [UsedImplicitly]
    internal class ListObjectsRequestMarshal : IRequestMarshal<ListObjectsRequest>
    {
        public Stream MarshalRequest(ListObjectsRequest request, IConfig config)
        {
            request.AddQueryParameter(AmzParameters.Delimiter, request.Delimiter);
            request.AddQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(AmzParameters.MaxKeys, request.MaxKeys);
            request.AddQueryParameter(AmzParameters.Prefix, request.Prefix);
            request.AddQueryParameter(AmzParameters.ContinuationToken, request.ContinuationToken);
            request.AddQueryParameter(AmzParameters.FetchOwner, request.FetchOwner);
            request.AddQueryParameter(AmzParameters.StartAfter, request.StartAfter);
            request.AddQueryParameter(AmzParameters.ListType, 2);
            return null;
        }
    }
}