using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Internals.Helpers;
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

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("DeleteResult");

                foreach (string name in XmlHelper.ReadElements(xmlReader))
                {
                    switch (name)
                    {
                        case "Deleted":
                            ParseDeleted(response, xmlReader);
                            break;
                        case "Error":
                            ParseError(response, xmlReader);
                            break;
                    }
                }
            }
        }

        private static void ParseDeleted(DeleteObjectsResponse response, XmlReader xmlReader)
        {
            string? key = null;
            string? versionId = null;
            bool deleteMarker = false;
            string? deleteVersionId = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Deleted"))
            {
                switch (name)
                {
                    case "Key":
                        key = xmlReader.ReadString();
                        break;
                    case "VersionId":
                        versionId = xmlReader.ReadString();
                        break;
                    case "DeleteMarker":
                        deleteMarker = ValueHelper.ParseBool(xmlReader.ReadString());
                        break;
                    case "DeleteMarkerVersionId":
                        deleteVersionId = xmlReader.ReadString();
                        break;
                }
            }

            if (key == null)
                throw new InvalidOperationException("Missing required values");

            response.Deleted.Add(new S3DeletedObject(key, versionId, deleteMarker, deleteVersionId));
        }

        private static void ParseError(DeleteObjectsResponse response, XmlReader xmlReader)
        {
            string? key = null;
            string? versionId = null;
            ErrorCode code = ErrorCode.Unknown;
            string? message = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Error"))
            {
                switch (name)
                {
                    case "Key":
                        key = xmlReader.ReadString();
                        break;
                    case "VersionId":
                        versionId = xmlReader.ReadString();
                        break;
                    case "Code":
                        code = ValueHelper.ParseEnum<ErrorCode>(xmlReader.ReadString());
                        break;
                    case "Message":
                        message = xmlReader.ReadString();
                        break;
                }
            }

            if (key == null || message == null)
                throw new InvalidOperationException("Missing required values");

            response.Errors.Add(new S3DeleteError(key, code, message, versionId));
        }
    }
}