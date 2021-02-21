using System.Net;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Extensions.AwsS3;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.AwsS3
{
    /// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
    public sealed class S3Client : ClientBase
    {
        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, byte[] accessKey, AwsRegion region, IWebProxy? proxy = null) : this(new AwsConfig(new AccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, string accessKey, AwsRegion region, IWebProxy? proxy = null) : this(new AwsConfig(new StringAccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="credentials">The credentials to use</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(IAccessKey credentials, AwsRegion region, IWebProxy? proxy = null) : this(new AwsConfig(credentials, region), proxy) { }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="config">The configuration you want to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(AwsConfig config, IWebProxy? proxy = null) : base(config, proxy) { }

        public S3Client(AwsConfig options, INetworkDriver networkDriver) : base(options, networkDriver) { }

        public S3Client(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer) : base(objectClient, bucketClient, multipartClient, multipartTransfer, transfer) { }
    }
}