using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Core.Responses.Objects.XML;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Core.Responses.XMLTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class DeleteMultipleObjectsResponseMarshal : IResponseMarshal<DeleteMultipleObjectsRequest, DeleteMultipleObjectsResponse>
    {
        public void MarshalResponse(DeleteMultipleObjectsRequest request, DeleteMultipleObjectsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
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
                        s3Deleted.Key = deleted.Key;
                        s3Deleted.DeleteMarkerVersionId = deleted.DeleteMarkerVersionId;
                        s3Deleted.VersionId = deleted.VersionId;
                        s3Deleted.DeleteMarker = deleted.DeleteMarker;

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
                        s3DeleteError.Key = error.Key;
                        s3DeleteError.VersionId = error.VersionId;
                        s3DeleteError.Code = error.Code;
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