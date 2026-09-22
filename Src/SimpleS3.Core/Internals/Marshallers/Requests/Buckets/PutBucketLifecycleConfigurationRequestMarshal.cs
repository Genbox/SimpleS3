using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketLifecycleConfigurationRequestMarshal : IRequestMarshal<PutBucketLifecycleConfigurationRequest>
{
    public Stream MarshalRequest(PutBucketLifecycleConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Lifecycle, string.Empty);

        if (request.TransitionDefaultMinimumObjectSize != TransitionDefaultMinimumObjectSize.Unknown)
            request.SetHeader("x-amz-transition-default-minimum-object-size", request.TransitionDefaultMinimumObjectSize.GetDisplayName());

        FastXmlWriter writer = new FastXmlWriter(600);
        writer.WriteStartElement("LifecycleConfiguration", XmlConstants.S3Namespace);

        foreach (S3Rule rule in request.Rules)
        {
            writer.WriteStartElement("Rule");

            if (rule.AbortIncompleteMultipartUploadDays.HasValue)
            {
                writer.WriteStartElement("AbortIncompleteMultipartUpload");
                writer.WriteElement("DaysAfterInitiation", rule.AbortIncompleteMultipartUploadDays.Value);
                writer.WriteEndElement("AbortIncompleteMultipartUpload");
            }

            if (rule.Expiration != null)
            {
                writer.WriteStartElement("Expiration");

                if (rule.Expiration.ExpireOnDate.HasValue)
                {
                    DateTimeOffset value = rule.Expiration.ExpireOnDate.Value.ToUniversalTime();
                    DateTimeOffset expirationDate = new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, TimeSpan.Zero);
                    writer.WriteElement("Date", ValueHelper.DateToString(expirationDate, DateTimeFormat.Iso8601DateTimeExt));
                }

                if (rule.Expiration.ExpireAfterDays.HasValue)
                    writer.WriteElement("Days", rule.Expiration.ExpireAfterDays.Value);

                if (rule.Expiration.ExpireObjectDeleteMarker.HasValue)
                    writer.WriteElement("ExpiredObjectDeleteMarker", rule.Expiration.ExpireObjectDeleteMarker.Value);

                writer.WriteEndElement("Expiration");
            }

            if (rule.Filter != null)
            {
                FilterHelper.Validate(rule.Filter, "Lifecycle", true);
                writer.WriteStartElement("Filter");

                if (rule.Filter.Prefix != null)
                    writer.WriteElement("Prefix", rule.Filter.Prefix);

                if (rule.Filter.Tag != null)
                {
                    writer.WriteStartElement("Tag");
                    writer.WriteElement("Key", rule.Filter.Tag.Value.Key);
                    writer.WriteElement("Value", rule.Filter.Tag.Value.Value);
                    writer.WriteEndElement("Tag");
                }

                S3AndCondition? andCondition = rule.Filter.AndConditions;

                if (rule.Filter.ObjectSizeGreaterThan.HasValue)
                    writer.WriteElement("ObjectSizeGreaterThan", XmlConvert.ToString(rule.Filter.ObjectSizeGreaterThan.Value));

                if (rule.Filter.ObjectSizeLessThan.HasValue)
                    writer.WriteElement("ObjectSizeLessThan", XmlConvert.ToString(rule.Filter.ObjectSizeLessThan.Value));

                if (andCondition != null)
                {
                    writer.WriteStartElement("And");

                    if (andCondition.Prefix != null)
                        writer.WriteElement("Prefix", andCondition.Prefix);

                    foreach (KeyValuePair<string, string> tag in andCondition.Tags)
                    {
                        writer.WriteStartElement("Tag");
                        writer.WriteElement("Key", tag.Key);
                        writer.WriteElement("Value", tag.Value);
                        writer.WriteEndElement("Tag");
                    }

                    if (andCondition.ObjectSizeGreaterThan.HasValue)
                        writer.WriteElement("ObjectSizeGreaterThan", XmlConvert.ToString(andCondition.ObjectSizeGreaterThan.Value));

                    if (andCondition.ObjectSizeLessThan.HasValue)
                        writer.WriteElement("ObjectSizeLessThan", XmlConvert.ToString(andCondition.ObjectSizeLessThan.Value));

                    writer.WriteEndElement("And");
                }

                writer.WriteEndElement("Filter");
            }

            if (rule.Id != null)
                writer.WriteElement("ID", rule.Id);

            if (rule.Prefix != null)
                writer.WriteElement("Prefix", rule.Prefix);

            if (rule.NonCurrentVersionExpirationDays.HasValue || rule.NonCurrentVersionExpirationNewerVersions.HasValue)
            {
                writer.WriteStartElement("NoncurrentVersionExpiration");
                if (rule.NonCurrentVersionExpirationDays.HasValue)
                    writer.WriteElement("NoncurrentDays", rule.NonCurrentVersionExpirationDays.Value);
                if (rule.NonCurrentVersionExpirationNewerVersions.HasValue)
                    writer.WriteElement("NewerNoncurrentVersions", rule.NonCurrentVersionExpirationNewerVersions.Value);
                writer.WriteEndElement("NoncurrentVersionExpiration");
            }

            foreach (S3NonCurrentVersionTransition transition in rule.NonCurrentVersionTransitions)
            {
                writer.WriteStartElement("NoncurrentVersionTransition");
                writer.WriteElement("NoncurrentDays", transition.NonCurrentDays);
                if (transition.NewerNonCurrentVersions.HasValue)
                    writer.WriteElement("NewerNoncurrentVersions", transition.NewerNonCurrentVersions.Value);
                writer.WriteElement("StorageClass", transition.StorageClass.GetDisplayName());
                writer.WriteEndElement("NoncurrentVersionTransition");
            }

            writer.WriteElement("Status", rule.Enabled ? "Enabled" : "Disabled");

            foreach (S3Transition transition in rule.Transitions)
            {
                writer.WriteStartElement("Transition");

                if (transition.TransitionOnDate.HasValue)
                {
                    DateTimeOffset value = transition.TransitionOnDate.Value.ToUniversalTime();
                    DateTimeOffset transitionDate = new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, TimeSpan.Zero);
                    writer.WriteElement("Date", ValueHelper.DateToString(transitionDate, DateTimeFormat.Iso8601DateTimeExt));
                }

                if (transition.TransitionAfterDays.HasValue)
                    writer.WriteElement("Days", transition.TransitionAfterDays.Value);

                if (transition.StorageClass != StorageClass.Unknown)
                    writer.WriteElement("StorageClass", transition.StorageClass.GetDisplayName());

                writer.WriteEndElement("Transition");
            }

            writer.WriteEndElement("Rule");
        }

        writer.WriteEndElement("LifecycleConfiguration");

        return new MemoryStream(writer.GetBytes());
    }
}