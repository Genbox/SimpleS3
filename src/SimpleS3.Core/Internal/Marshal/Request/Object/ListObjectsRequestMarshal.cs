using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class ListObjectsRequestMarshal : IRequestMarshal<ListObjectsRequest>
    {
        public Stream MarshalRequest(ListObjectsRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.Delimiter, request.Delimiter);
            request.AddQueryParameter(AmzParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(AmzParameters.MaxKeys, request.MaxKeys);
            request.AddQueryParameter(AmzParameters.Prefix, request.Prefix);
            request.AddQueryParameter(AmzParameters.ContinuationToken, request.ContinuationToken);
            request.AddQueryParameter(AmzParameters.FetchOwner, request.FetchOwner);
            request.AddQueryParameter(AmzParameters.StartAfter, request.StartAfter);
            request.AddQueryParameter(AmzParameters.ListType, 2);
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);
            return null;
        }
    }
}