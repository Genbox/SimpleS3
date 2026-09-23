using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class ListBucketsResponseMarshal : IResponseMarshal<ListBucketsResponse>
{
    public void MarshalResponse(SimpleS3Config config, ListBucketsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);
        XmlHelper.ReadToDescendant(xmlReader, "ListAllMyBucketsResult");

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            switch (name)
            {
                case "Owner":
                    response.Owner = ParserHelper.ParseOwner(xmlReader);
                    break;
                case "Buckets":
                    ReadBuckets(response, xmlReader);
                    break;
                case "ContinuationToken":
                    response.ContinuationToken = xmlReader.ReadString();
                    break;
                case "Prefix":
                    response.Prefix = xmlReader.ReadString();
                    break;
            }
        }
    }

    private static void ReadBuckets(ListBucketsResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "Buckets"))
        {
            if (name == "Bucket")
                ReadBucket(response, xmlReader);
        }
    }

    private static void ReadBucket(ListBucketsResponse response, XmlReader xmlReader)
    {
        string? bucketName = null;
        DateTimeOffset? creationDate = null;
        string? bucketRegion = null;
        string? bucketArn = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Bucket"))
        {
            switch (name)
            {
                case "Name":
                    bucketName = xmlReader.ReadString();
                    break;
                case "CreationDate":
                    creationDate = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
                case "BucketRegion":
                    bucketRegion = xmlReader.ReadString();
                    break;
                case "BucketArn":
                    bucketArn = xmlReader.ReadString();
                    break;
            }
        }

        if (bucketName == null || creationDate == null)
            throw new InvalidOperationException("Missing required values");

        response.Buckets.Add(new S3Bucket(bucketName, creationDate.Value, bucketRegion, bucketArn));
    }
}