using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    internal class GetBucketLifecycleConfigurationResponseMarshal : IResponseMarshal<GetBucketLifecycleConfigurationResponse>
    {
        public void MarshalResponse(SimpleS3Config config, GetBucketLifecycleConfigurationResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("LifecycleConfiguration");

                foreach (string name in XmlHelper.ReadElements(xmlReader))
                {
                    switch (name)
                    {
                        case "Rule":
                            ReadRule(response, xmlReader);
                            break;
                    }
                }
            }
        }

        private static void ReadRule(GetBucketLifecycleConfigurationResponse response, XmlReader xmlReader)
        {
            string? id = null;
            bool? status = null;
            int? abortMultipartDays = null;
            int? nonCurrentDays = null;
            IList<S3NonCurrentVersionTransition>? nonCurrentVersionTransitions = null;
            S3Expiration? expiration = null;
            S3Filter? filter = null;
            IList<S3Transition>? transitions = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Rule"))
            {
                switch (name)
                {
                    case "ID":
                        id = xmlReader.ReadString();
                        break;
                    case "Status":
                        status = xmlReader.ReadString() == "Enabled";
                        break;
                    case "AbortIncompleteMultipartUpload":
                        xmlReader.ReadToDescendant("DaysAfterInitiation");
                        abortMultipartDays = ValueHelper.ParseInt(xmlReader.ReadString());
                        break;
                    case "Expiration":
                        expiration = ReadExpiration(xmlReader);
                        break;
                    case "Filter":
                        filter = ReadFilter(xmlReader);
                        break;
                    case "NoncurrentVersionExpiration":
                        xmlReader.ReadToDescendant("NoncurrentDays");
                        abortMultipartDays = ValueHelper.ParseInt(xmlReader.ReadString());
                        break;
                    case "NoncurrentVersionTransition":
                        nonCurrentVersionTransitions ??= new List<S3NonCurrentVersionTransition>();
                        ReadNonCurrentVersionTransition(nonCurrentVersionTransitions, xmlReader);
                        break;
                    case "Transition":
                        transitions ??= new List<S3Transition>();
                        ReadTransition(transitions, xmlReader);
                        break;
                }
            }

            if (id == null || status == null)
                throw new InvalidOperationException("Missing required values");

            S3Rule rule = new S3Rule(id, status.Value);
            rule.AbortIncompleteMultipartUploadDays = abortMultipartDays;
            rule.Expiration = expiration;
            rule.Filter = filter;
            rule.NonCurrentVersionExpirationDays = nonCurrentDays;

            if (nonCurrentVersionTransitions != null)
            {
                foreach (S3NonCurrentVersionTransition transition in nonCurrentVersionTransitions)
                {
                    rule.NonCurrentVersionTransitions.Add(transition);
                }
            }

            if (transitions != null)
            {
                foreach (S3Transition transition in transitions)
                {
                    rule.Transitions.Add(transition);
                }
            }

            response.Rules.Add(rule);
        }

        private static S3Expiration ReadExpiration(XmlReader xmlReader)
        {
            DateTimeOffset? date = null;
            int? days = null;
            bool? expiredObjectDeleteMarker = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Expiration"))
            {
                switch (name)
                {
                    case "Date":
                        date = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                    case "Days":
                        days = ValueHelper.ParseInt(xmlReader.ReadString());
                        break;
                    case "ExpiredObjectDeleteMarker":
                        expiredObjectDeleteMarker = ValueHelper.ParseBool(xmlReader.ReadString());
                        break;
                }
            }

            if (date == null && days == null && expiredObjectDeleteMarker == null)
                throw new InvalidOperationException("Missing required values");

            return new S3Expiration(date, days, expiredObjectDeleteMarker);
        }

        private static S3Filter ReadFilter(XmlReader xmlReader)
        {
            string? prefix = null;
            KeyValuePair<string, string>? tag = null;
            S3AndCondition? andCondition = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Filter"))
            {
                switch (name)
                {
                    case "Prefix":
                        prefix = xmlReader.ReadString();
                        break;
                    case "Tag":
                        tag = ReadTag(xmlReader);
                        break;
                    case "And":
                        andCondition = ReadAndCondition(xmlReader);
                        break;
                }
            }

            //It is allowed to have an empty filter. It means the whole bucket is affected.

            S3Filter filter = new S3Filter();
            filter.Prefix = prefix;
            filter.Tag = tag;
            filter.AndConditions = andCondition;

            return filter;
        }

        private static S3AndCondition ReadAndCondition(XmlReader xmlReader)
        {
            string? prefix = null;
            IList<KeyValuePair<string, string>> tags = new List<KeyValuePair<string, string>>();

            foreach (string name in XmlHelper.ReadElements(xmlReader, "And"))
            {
                switch (name)
                {
                    case "Prefix":
                        prefix = xmlReader.ReadString();
                        break;
                    case "Tag":
                        tags.Add(ReadTag(xmlReader));
                        break;
                }
            }

            if (prefix == null && tags.Count == 0)
                throw new InvalidOperationException("Missing required values");

            S3AndCondition condition = new S3AndCondition();
            condition.Prefix = prefix;
            condition.Tags = tags;
            return condition;
        }

        private static KeyValuePair<string, string> ReadTag(XmlReader xmlReader)
        {
            string? key = null;
            string? value = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Tag"))
            {
                switch (name)
                {
                    case "Key":
                        key = xmlReader.ReadString();
                        break;
                    case "Value":
                        value = xmlReader.ReadString();
                        break;
                }
            }

            if (key == null || value == null)
                throw new InvalidOperationException("Missing required values");

            return new KeyValuePair<string, string>(key, value);
        }

        private static void ReadNonCurrentVersionTransition(IList<S3NonCurrentVersionTransition> list, XmlReader xmlReader)
        {
            int? nonCurrentDays = null;
            StorageClass storageClass = StorageClass.Unknown;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "NoncurrentVersionTransition"))
            {
                switch (name)
                {
                    case "NoncurrentDays":
                        nonCurrentDays = ValueHelper.ParseInt(xmlReader.ReadString());
                        break;
                    case "StorageClass":
                        storageClass = ValueHelper.ParseEnum<StorageClass>(xmlReader.ReadString());
                        break;
                }
            }

            if (nonCurrentDays == null || storageClass == StorageClass.Unknown)
                throw new InvalidOperationException("Missing required values");

            list.Add(new S3NonCurrentVersionTransition(nonCurrentDays.Value, storageClass));
        }

        private static void ReadTransition(IList<S3Transition> list, XmlReader xmlReader)
        {
            DateTimeOffset? date = null;
            int? days = null;
            StorageClass storageClass = StorageClass.Unknown;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Transition"))
            {
                switch (name)
                {
                    case "Date":
                        date = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                    case "Days":
                        days = ValueHelper.ParseInt(xmlReader.ReadString());
                        break;
                    case "StorageClass":
                        storageClass = ValueHelper.ParseEnum<StorageClass>(xmlReader.ReadString());
                        break;
                }
            }

            if (date == null && days == null || storageClass == StorageClass.Unknown)
                throw new InvalidOperationException("Missing required values");

            list.Add(new S3Transition(date, days, storageClass));
        }
    }
}