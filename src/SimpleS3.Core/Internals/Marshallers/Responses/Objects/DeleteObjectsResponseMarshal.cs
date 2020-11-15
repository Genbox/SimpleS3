using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.Xml;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class DeleteObjectsResponseMarshal : IResponseMarshal<DeleteObjectsResponse>
    {
        public void MarshalResponse(Config config, DeleteObjectsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            XmlSerializer s = new XmlSerializer(typeof(DeleteResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                DeleteResult deleteResult = (DeleteResult)s.Deserialize(r);

                if (deleteResult.Deleted != null)
                {
                    response.Deleted = new List<S3DeletedObject>(deleteResult.Deleted.Count);

                    foreach (Deleted deleted in deleteResult.Deleted)
                    {
                        S3DeletedObject s3Deleted = new S3DeletedObject();
                        s3Deleted.ObjectKey = deleted.Key;
                        s3Deleted.DeleteMarkerVersionId = deleted.DeleteMarkerVersionId;
                        s3Deleted.VersionId = deleted.VersionId;
                        s3Deleted.IsDeleteMarker = deleted.DeleteMarker;

                        response.Deleted.Add(s3Deleted);
                    }
                }
                else
                    response.Deleted = Array.Empty<S3DeletedObject>();

                if (deleteResult.Error != null)
                {
                    response.Errors = new List<S3DeleteError>(deleteResult.Error.Count);

                    foreach (Error error in deleteResult.Error)
                    {
                        S3DeleteError s3DeleteError = new S3DeleteError();
                        s3DeleteError.ObjectKey = error.Key;
                        s3DeleteError.VersionId = error.VersionId;
                        s3DeleteError.Code = ValueHelper.ParseEnum<ErrorCode>(error.Code);
                        s3DeleteError.Message = error.Message;

                        response.Errors.Add(s3DeleteError);
                    }
                }
                else
                    response.Errors = Array.Empty<S3DeleteError>();
            }
        }
    }
}