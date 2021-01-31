using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    [UsedImplicitly]
    internal class ListBucketsResponseMarshal : IResponseMarshal<ListBucketsResponse>
    {
        public void MarshalResponse(Config config, ListBucketsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("ListAllMyBucketsResult");

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    switch (xmlReader.Name)
                    {
                        case "Owner":
                            ReadOwner(response, xmlReader);
                            break;
                        case "Buckets":
                            ReadBuckets(response, xmlReader);
                            break;
                    }
                }
            }
        }

        private void ReadOwner(ListBucketsResponse response, XmlTextReader xmlReader)
        {
            string? id = null;
            string? displayName = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Owner")
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (xmlReader.Name)
                {
                    case "ID":
                        id = xmlReader.ReadString();
                        break;
                    case "DisplayName":
                        displayName = xmlReader.ReadString();
                        break;
                }
            }

            if (id == null || displayName == null)
                throw new InvalidOperationException("Missing required values");

            response.Owner = new S3Identity(id, displayName);
        }


        private void ReadBuckets(ListBucketsResponse response, XmlTextReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Buckets")
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                if (xmlReader.Name == "Bucket")
                    ReadBucket(response, xmlReader);
            }
        }

        private void ReadBucket(ListBucketsResponse response, XmlTextReader xmlReader)
        {
            string? name = null;
            DateTimeOffset? creationData = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Bucket")
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (xmlReader.Name)
                {
                    case "Name":
                        name = xmlReader.ReadString();
                        break;
                    case "CreationDate":
                        creationData = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                }
            }

            if (name == null || creationData == null)
                throw new InvalidOperationException("Missing required values");

            response.Buckets.Add(new S3Bucket(name, creationData.Value));
        }
    }
}