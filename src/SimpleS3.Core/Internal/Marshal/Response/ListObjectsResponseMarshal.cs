using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Buckets.XML;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XMLTypes;
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
                response.Delimiter = bucketResult.Delimiter;
                response.IsTruncated = bucketResult.IsTruncated;
                response.KeyCount = bucketResult.KeyCount;
                response.BucketName = bucketResult.Name;
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
                        obj.ObjectKey = content.Key;
                        obj.ETag = content.ETag;

                        if (content.StorageClass != null)
                            obj.StorageClass = ValueHelper.ParseEnum<StorageClass>(content.StorageClass);

                        obj.LastModifiedOn = content.LastModified;
                        obj.Size = content.Size;

                        if (content.Owner != null)
                            obj.Owner = new S3Identity { Name = content.Owner.DisplayName, Id = content.Owner.Id };

                        response.Objects.Add(obj);
                    }
                }
                else
                    response.Objects = Array.Empty<S3Object>();
            }
        }
    }
}