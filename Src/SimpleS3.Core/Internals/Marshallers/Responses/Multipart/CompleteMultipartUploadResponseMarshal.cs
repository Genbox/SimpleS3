using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;

internal sealed class CompleteMultipartUploadResponseMarshal : IResponseMarshal<CompleteMultipartUploadResponse>
{
    public void MarshalResponse(SimpleS3Config config, CompleteMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSse, out string? sseHeader))
            response.SseAlgorithm = Core.Enums.Enums.SseAlgorithm.Parse(sseHeader, SseAlgorithmFormat.DisplayName);

        response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSseCustomerAlgorithm, out string? sseAlgorithm))
            response.SseCustomerAlgorithm = Core.Enums.Enums.SseCustomerAlgorithm.Parse(sseAlgorithm, SseCustomerAlgorithmFormat.DisplayName);

        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        if (ParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
        {
            response.LifeCycleExpiresOn = data.expiresOn;
            response.LifeCycleRuleId = data.ruleId;
        }

        using XmlTextReader xmlReader = new XmlTextReader(responseStream);
        xmlReader.ReadToDescendant("CompleteMultipartUploadResult");

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            switch (name)
            {
                case "Location":
                    response.Location = xmlReader.ReadString();
                    break;
                case "Bucket":
                    response.BucketName = xmlReader.ReadString();
                    break;
                case "Key":
                    response.ObjectKey = xmlReader.ReadString();
                    break;
                case "ETag":
                    response.ETag = xmlReader.ReadString();
                    break;
            }
        }
    }
}