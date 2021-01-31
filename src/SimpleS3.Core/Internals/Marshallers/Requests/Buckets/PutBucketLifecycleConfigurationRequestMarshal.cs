using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class PutBucketLifecycleConfigurationRequestMarshal : IRequestMarshal<PutBucketLifecycleConfigurationRequest>
    {
        public Stream? MarshalRequest(PutBucketLifecycleConfigurationRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.Lifecycle, string.Empty);

            FastXmlWriter writer = new FastXmlWriter(600);
            writer.WriteStartElement("LifecycleConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");

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
                        writer.WriteElement("Date", ValueHelper.DateToString(rule.Expiration.ExpireOnDate.Value.UtcDateTime.Date, DateTimeFormat.Iso8601DateTimeExt));

                    if (rule.Expiration.ExpireAfterDays.HasValue)
                        writer.WriteElement("Days", rule.Expiration.ExpireAfterDays.Value);

                    if (rule.Expiration.ExpireObjectDeleteMarker.HasValue)
                        writer.WriteElement("ExpiredObjectDeleteMarker", rule.Expiration.ExpireObjectDeleteMarker.Value);

                    writer.WriteEndElement("Expiration");
                }

                if (rule.Filter != null)
                {
                    if (rule.Filter.Prefix != null)
                        writer.WriteElement("Prefix", rule.Filter.Prefix);

                    if (rule.Filter.Tag != null)
                    {
                        writer.WriteStartElement("Tag");
                        writer.WriteElement("Key", rule.Filter.Tag.Value.Key);
                        writer.WriteElement("Value", rule.Filter.Tag.Value.Value);
                        writer.WriteEndElement("Tag");
                    }

                    foreach (S3AndCondition condition in rule.Filter.Conditions)
                    {
                        writer.WriteStartElement("And");

                        if (condition.Prefix != null)
                            writer.WriteElement("Prefix", condition.Prefix);

                        if (condition.Tag != null)
                        {
                            writer.WriteStartElement("Tag");
                            writer.WriteElement("Key", condition.Tag.Value.Key);
                            writer.WriteElement("Value", condition.Tag.Value.Value);
                            writer.WriteEndElement("Tag");
                        }

                        writer.WriteEndElement("And");
                    }
                }

                writer.WriteElement("ID", rule.Id);

                if (rule.NonCurrentVersionExpirationDays != null)
                {
                    writer.WriteStartElement("NoncurrentVersionExpiration");
                    writer.WriteElement("NoncurrentDays", rule.NonCurrentVersionExpirationDays.Value);
                    writer.WriteEndElement("NoncurrentVersionExpiration");
                }

                foreach (S3NonCurrentVersionTransition transition in rule.NonCurrentVersionTransitions)
                {
                    writer.WriteStartElement("NoncurrentVersionTransition");
                    writer.WriteElement("NoncurrentDays", transition.NonCurrentDays);
                    writer.WriteElement("StorageClass", ValueHelper.EnumToString(transition.StorageClass));
                    writer.WriteEndElement("NoncurrentVersionTransition");
                }

                writer.WriteElement("Status", rule.Enabled ? "Enabled" : "Disabled");

                foreach (S3Transition transition in rule.Transitions)
                {
                    writer.WriteStartElement("Transition");

                    if (transition.TransitionOnDate.HasValue)
                        writer.WriteElement("Date", ValueHelper.DateToString(transition.TransitionOnDate.Value, DateTimeFormat.Iso8601Date));

                    if (transition.TransitionAfterDays.HasValue)
                        writer.WriteElement("Days", transition.TransitionAfterDays.Value);

                    if (transition.StorageClass != StorageClass.Unknown)
                        writer.WriteElement("StorageClass", ValueHelper.EnumToString(transition.StorageClass));

                    writer.WriteEndElement("Transition");
                }

                writer.WriteEndElement("Rule");
            }

            writer.WriteEndElement("LifecycleConfiguration");

            return new MemoryStream(writer.GetBytes());
        }
    }
}