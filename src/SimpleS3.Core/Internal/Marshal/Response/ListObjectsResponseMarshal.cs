using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets.XML;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Core.Responses.XMLTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class ListObjectsResponseMarshal : IResponseMarshal<ListObjectsRequest, ListObjectsResponse>
    {
        public void MarshalResponse(ListObjectsRequest request, ListObjectsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            XmlSerializer s = new XmlSerializer(typeof(ListBucketResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListBucketResult bucketResult = (ListBucketResult)s.Deserialize(r);
                response.MaxKeys = bucketResult.MaxKeys;
                response.IsTruncated = bucketResult.IsTruncated;
                response.KeyCount = bucketResult.KeyCount;
                response.Name = bucketResult.Name;
                response.Prefix = bucketResult.Prefix;
                response.ContinuationToken = bucketResult.ContinuationToken;

                if (bucketResult.CommonPrefixes != null)
                {
                    response.CommonPrefixes = new List<string>(bucketResult.CommonPrefixes.Count);

                    foreach (CommonPrefix prefix in bucketResult.CommonPrefixes)
                        response.CommonPrefixes.Add(prefix.Prefix);
                }
                else
                    response.CommonPrefixes = Array.Empty<string>();

                if (bucketResult.EncodingType != null)
                    response.EncodingType = ValueHelper.ParseEnum<EncodingType>(bucketResult.EncodingType);

                response.NextContinuationToken = bucketResult.NextContinuationToken;
                response.StartAfter = bucketResult.StartAfter;

                if (bucketResult.Contents != null)
                {
                    response.Objects = new List<S3Object>(bucketResult.KeyCount);

                    foreach (Content content in bucketResult.Contents)
                    {
                        S3Object obj = new S3Object();
                        obj.Name = content.Key;
                        obj.ETag = content.ETag;

                        if (content.StorageClass != null)
                            obj.StorageClass = ValueHelper.ParseEnum<StorageClass>(content.StorageClass);

                        obj.LastModified = content.LastModified;
                        obj.Size = content.Size;

                        if (content.Owner != null)
                            obj.Owner = new S3ObjectIdentity { Name = content.Owner.DisplayName, Id = content.Owner.Id };

                        response.Objects.Add(obj);
                    }
                }
                else
                    response.Objects = Array.Empty<S3Object>();
            }
        }
    }
}