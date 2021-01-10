using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Network.Xml;
using Genbox.SimpleS3.Core.Internals.Network.XmlTypes;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class ListObjectsResponseMarshal : IResponseMarshal<ListObjectsResponse>
    {
        public void MarshalResponse(Config config, ListObjectsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            XmlSerializer s = new XmlSerializer(typeof(ListBucketResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListBucketResult bucketResult = (ListBucketResult)s.Deserialize(r);

                if (bucketResult.EncodingType != null)
                    response.EncodingType = ValueHelper.ParseEnum<EncodingType>(bucketResult.EncodingType);

                response.Delimiter = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(bucketResult.Delimiter) : bucketResult.Delimiter;
                response.Prefix = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(bucketResult.Prefix) : bucketResult.Prefix;
                response.StartAfter = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(bucketResult.StartAfter) : bucketResult.StartAfter;
                response.NextContinuationToken = bucketResult.NextContinuationToken;
                response.MaxKeys = bucketResult.MaxKeys;
                response.IsTruncated = bucketResult.IsTruncated;
                response.KeyCount = bucketResult.KeyCount;
                response.BucketName = bucketResult.Name;
                response.ContinuationToken = bucketResult.ContinuationToken;
                response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

                if (bucketResult.CommonPrefixes != null)
                {
                    response.CommonPrefixes = new List<string>(bucketResult.CommonPrefixes.Count);

                    foreach (CommonPrefix prefix in bucketResult.CommonPrefixes)
                    {
                        response.CommonPrefixes.Add(prefix.Prefix);
                    }
                }
                else
                    response.CommonPrefixes = Array.Empty<string>();

                if (bucketResult.Contents != null)
                {
                    response.Objects = new List<S3Object>(bucketResult.KeyCount);

                    foreach (Content content in bucketResult.Contents)
                    {
                        string objectKey = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(content.Key) : content.Key;

                        StorageClass storageClass = StorageClass.Unknown;

                        if (content.StorageClass != null)
                            storageClass = ValueHelper.ParseEnum<StorageClass>(content.StorageClass);

                        S3Identity? owner = null;

                        if (content.Owner != null)
                            owner = new S3Identity(content.Owner.Id, content.Owner.DisplayName);

                        response.Objects.Add(new S3Object(objectKey, content.LastModified, content.Size, owner, content.ETag, storageClass));
                    }
                }
                else
                    response.Objects = Array.Empty<S3Object>();
            }
        }
    }
}