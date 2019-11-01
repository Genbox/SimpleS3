using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.XML;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XMLTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class ListPartsResponseMarshal : IResponseMarshal<ListPartsRequest, ListPartsResponse>
    {
        public void MarshalResponse(IS3Config config, ListPartsRequest request, ListPartsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = string.Equals("RequestPayer", headers.GetHeader(AmzHeaders.XAmzVersionId), StringComparison.OrdinalIgnoreCase);
            response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Iso8601DateTimeExt);
            response.AbortRuleId = headers.GetHeader(AmzHeaders.XAmzAbortDate);

            XmlSerializer s = new XmlSerializer(typeof(ListPartsResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListPartsResult listResult = (ListPartsResult)s.Deserialize(r);
                response.BucketName = listResult.Bucket;
                response.ObjectKey = listResult.Key;
                response.UploadId = listResult.UploadId;

                if (listResult.StorageClass != null)
                    response.StorageClass = ValueHelper.ParseEnum<StorageClass>(listResult.StorageClass);

                response.PartNumberMarker = listResult.PartNumberMarker;
                response.NextPartNumberMarker = listResult.NextPartNumberMarker;
                response.MaxParts = listResult.MaxParts;
                response.IsTruncated = listResult.IsTruncated;

                if (listResult.EncodingType != null)
                    response.EncodingType = ValueHelper.ParseEnum<EncodingType>(listResult.EncodingType);

                if (listResult.Owner != null)
                {
                    response.Owner = new S3Identity();
                    response.Owner.Name = listResult.Owner.DisplayName;
                    response.Owner.Id = listResult.Owner.Id;
                }

                if (listResult.Initiator != null)
                {
                    response.Initiator = new S3Identity();
                    response.Initiator.Name = listResult.Initiator.DisplayName;
                    response.Initiator.Id = listResult.Initiator.Id;
                }

                if (listResult.Part != null)
                {
                    response.Parts = new List<S3Part>(listResult.Part.Count);

                    foreach (Part part in listResult.Part)
                    {
                        S3Part s3Part = new S3Part();
                        s3Part.ETag = part.ETag;
                        s3Part.LastModified = part.LastModified;
                        s3Part.PartNumber = part.PartNumber;
                        s3Part.Size = part.Size;

                        response.Parts.Add(s3Part);
                    }
                }
                else
                    response.Parts = Array.Empty<S3Part>();
            }
        }
    }
}