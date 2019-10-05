using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using EnumsNET;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets.XML;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Core.Responses.XMLTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class ListMultipartUploadsResponseMarshal : IResponseMarshal<ListMultipartUploadsRequest, ListMultipartUploadsResponse>
    {
        public void MarshalResponse(ListMultipartUploadsRequest request, ListMultipartUploadsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            XmlSerializer s = new XmlSerializer(typeof(ListMultipartUploadsResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListMultipartUploadsResult listResult = (ListMultipartUploadsResult)s.Deserialize(r);
                response.Bucket = listResult.Bucket;
                response.KeyMarker = listResult.KeyMarker;
                response.UploadIdMarker = listResult.UploadIdMarker;
                response.NextKeyMarker = listResult.NextKeyMarker;
                response.NextUploadIdMarker = listResult.NextUploadIdMarker;

                if (listResult.EncodingType != null)
                    response.EncodingType = EnumsNET.Enums.Parse<EncodingType>(listResult.EncodingType, EnumFormat.EnumMemberValue);

                response.MaxUploads = listResult.MaxUploads;
                response.IsTruncated = listResult.IsTruncated;
                response.Prefix = listResult.Prefix;
                response.Delimiter = listResult.Delimiter;

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
                        s3Upload.Name = listUpload.Key;
                        s3Upload.UploadId = listUpload.UploadId;

                        if (listUpload.StorageClass != null)
                            s3Upload.StorageClass = EnumsNET.Enums.Parse<StorageClass>(listUpload.StorageClass, EnumFormat.EnumMemberValue);

                        s3Upload.Initiated = listUpload.Initiated;

                        if (listUpload.Owner != null)
                        {
                            s3Upload.Owner = new S3ObjectIdentity();
                            s3Upload.Owner.Name = listUpload.Owner.DisplayName;
                            s3Upload.Owner.Id = listUpload.Owner.Id;
                        }

                        if (listUpload.Initiator != null)
                        {
                            s3Upload.Initiator = new S3ObjectIdentity();
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