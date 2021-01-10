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
using Genbox.SimpleS3.Core.Internals.Network.Xml;
using Genbox.SimpleS3.Core.Internals.Network.XmlTypes;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
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
                        response.Deleted.Add(new S3DeletedObject(deleted.Key, deleted.DeleteMarkerVersionId, deleted.DeleteMarker, deleted.VersionId));
                    }
                }
                else
                    response.Deleted = Array.Empty<S3DeletedObject>();

                if (deleteResult.Error != null)
                {
                    response.Errors = new List<S3DeleteError>(deleteResult.Error.Count);

                    foreach (Error error in deleteResult.Error)
                    {
                        response.Errors.Add(new S3DeleteError(error.Key, ValueHelper.ParseEnum<ErrorCode>(error.Code), error.Message, error.VersionId));
                    }
                }
                else
                    response.Errors = Array.Empty<S3DeleteError>();
            }
        }
    }
}