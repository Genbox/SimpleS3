using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart.Xml;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class ListPartsResponseMarshal : IResponseMarshal<ListPartsResponse>
    {
        public void MarshalResponse(Config config, ListPartsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Iso8601DateTimeExt);
            response.AbortRuleId = headers.GetOptionalValue(AmzHeaders.XAmzAbortRuleId);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            XmlSerializer s = new XmlSerializer(typeof(ListPartsResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListPartsResult listResult = (ListPartsResult)s.Deserialize(r);

                if (listResult.EncodingType != null)
                    response.EncodingType = ValueHelper.ParseEnum<EncodingType>(listResult.EncodingType);

                response.ObjectKey = config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url ? WebUtility.UrlDecode(listResult.Key) : listResult.Key;
                response.BucketName = listResult.Bucket;
                response.UploadId = listResult.UploadId;

                if (listResult.StorageClass != null)
                    response.StorageClass = ValueHelper.ParseEnum<StorageClass>(listResult.StorageClass);

                response.PartNumberMarker = listResult.PartNumberMarker;
                response.NextPartNumberMarker = listResult.NextPartNumberMarker;
                response.MaxParts = listResult.MaxParts;
                response.IsTruncated = listResult.IsTruncated;

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

                if (listResult.Parts != null)
                {
                    response.Parts = new List<S3Part>(listResult.Parts.Count);

                    foreach (Part part in listResult.Parts)
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