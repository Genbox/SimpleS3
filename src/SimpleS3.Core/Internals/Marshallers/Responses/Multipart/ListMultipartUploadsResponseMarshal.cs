using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Buckets.Xml;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class ListMultipartUploadsResponseMarshal : IResponseMarshal<ListMultipartUploadsRequest, ListMultipartUploadsResponse>
    {
        public void MarshalResponse(IS3Config config, ListMultipartUploadsRequest request, ListMultipartUploadsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            XmlSerializer s = new XmlSerializer(typeof(ListMultipartUploadsResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListMultipartUploadsResult listResult = (ListMultipartUploadsResult)s.Deserialize(r);

                if (listResult.EncodingType != null)
                    response.EncodingType = ValueHelper.ParseEnum<EncodingType>(listResult.EncodingType);

                response.KeyMarker = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(listResult.KeyMarker) : listResult.KeyMarker;
                response.NextKeyMarker = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(listResult.NextKeyMarker) : listResult.NextKeyMarker;
                response.Prefix = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(listResult.Prefix) : listResult.Prefix;
                response.Delimiter = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(listResult.Delimiter) : listResult.Delimiter;
                response.Bucket = listResult.Bucket;
                response.UploadIdMarker = listResult.UploadIdMarker;
                response.NextUploadIdMarker = listResult.NextUploadIdMarker;
                response.MaxUploads = listResult.MaxUploads;
                response.IsTruncated = listResult.IsTruncated;

                if (listResult.CommonPrefixes != null)
                {
                    response.CommonPrefixes = new List<string>(listResult.CommonPrefixes.Count);

                    foreach (CommonPrefix prefix in listResult.CommonPrefixes)
                        response.CommonPrefixes.Add(prefix.Prefix);
                }
                else
                    response.CommonPrefixes = Array.Empty<string>();

                if (listResult.Uploads != null)
                {
                    response.Uploads = new List<S3Upload>(listResult.Uploads.Count);

                    foreach (Upload listUpload in listResult.Uploads)
                    {
                        S3Upload s3Upload = new S3Upload();
                        s3Upload.ObjectKey = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(listUpload.Key) : listUpload.Key;
                        s3Upload.UploadId = listUpload.UploadId;

                        if (listUpload.StorageClass != null)
                            s3Upload.StorageClass = ValueHelper.ParseEnum<StorageClass>(listUpload.StorageClass);

                        s3Upload.Initiated = listUpload.Initiated;

                        if (listUpload.Owner != null)
                        {
                            s3Upload.Owner = new S3Identity();
                            s3Upload.Owner.Name = listUpload.Owner.DisplayName;
                            s3Upload.Owner.Id = listUpload.Owner.Id;
                        }

                        if (listUpload.Initiator != null)
                        {
                            s3Upload.Initiator = new S3Identity();
                            s3Upload.Initiator.Name = listUpload.Initiator.DisplayName;
                            s3Upload.Initiator.Id = listUpload.Initiator.Id;
                        }

                        response.Uploads.Add(s3Upload);
                    }
                }
                else
                    response.Uploads = Array.Empty<S3Upload>();
            }
        }
    }
}