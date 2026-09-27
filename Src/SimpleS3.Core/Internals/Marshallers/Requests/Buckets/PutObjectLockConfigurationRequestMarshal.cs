using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutObjectLockConfigurationRequestMarshal : IRequestMarshal<PutObjectLockConfigurationRequest>
{
    public Stream MarshalRequest(PutObjectLockConfigurationRequest request, SimpleS3Config config)
    {
        if (!request.Enabled)
            throw new ArgumentException("Object Lock cannot be disabled. Use ClearBucketDefaultRetentionAsync to remove the default retention rule.", nameof(request));

        bool hasDays = request.LockRetainDays.HasValue;
        bool hasYears = request.LockRetainYears.HasValue;
        bool hasMode = request.LockMode != LockMode.Unknown;
        if (hasMode ? hasDays == hasYears : hasDays || hasYears)
            throw new ArgumentException("Default retention requires a mode and exactly one duration in days or years.", nameof(request));
        if (request.LockRetainDays <= 0 || request.LockRetainYears <= 0)
            throw new ArgumentException("Default retention duration must be greater than zero.", nameof(request));

        request.SetQueryParameter(AmzParameters.ObjectLock, string.Empty);
        if (request.LockToken != null)
            request.SetHeader("x-amz-bucket-object-lock-token", request.LockToken);

        FastXmlWriter writer = new FastXmlWriter(128);
        writer.WriteStartElement("ObjectLockConfiguration", XmlConstants.S3Namespace);
        writer.WriteElement("ObjectLockEnabled", "Enabled");

        if (request.LockMode != LockMode.Unknown)
        {
            writer.WriteStartElement("Rule");
            writer.WriteStartElement("DefaultRetention");
            writer.WriteElement("Mode", request.LockMode.GetDisplayName());

            if (hasDays)
                writer.WriteElement("Days", request.LockRetainDays!.Value);
            if (hasYears)
                writer.WriteElement("Years", request.LockRetainYears!.Value);

            writer.WriteEndElement("DefaultRetention");
            writer.WriteEndElement("Rule");
        }

        writer.WriteEndElement("ObjectLockConfiguration");

        return new MemoryStream(writer.GetBytes());
    }
}