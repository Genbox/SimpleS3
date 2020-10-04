using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets.Xml;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    [UsedImplicitly]
    internal class ListBucketsResponseMarshal : IResponseMarshal<ListBucketsResponse>
    {
        public void MarshalResponse(Config config, ListBucketsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            XmlSerializer s = new XmlSerializer(typeof(ListAllMyBucketsResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                ListAllMyBucketsResult listResult = (ListAllMyBucketsResult)s.Deserialize(r);

                if (listResult.Owner != null)
                {
                    response.Owner = new S3Identity();
                    response.Owner.Id = listResult.Owner.Id;
                    response.Owner.Name = listResult.Owner.DisplayName;
                }

                if (listResult.Buckets != null)
                {
                    response.Buckets = new List<S3Bucket>(listResult.Buckets.Count);

                    foreach (Bucket lb in listResult.Buckets)
                    {
                        S3Bucket b = new S3Bucket();
                        b.Name = lb.Name;
                        b.CreatedOn = lb.CreationDate;

                        response.Buckets.Add(b);
                    }
                }
                else
                    response.Buckets = Array.Empty<S3Bucket>();
            }
        }
    }
}