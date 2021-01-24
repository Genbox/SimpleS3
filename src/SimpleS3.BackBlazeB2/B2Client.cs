using System.Net;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Extensions.BackBlazeB2;

namespace Genbox.SimpleS3.BackBlazeB2
{
    /// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
    public sealed class B2Client : ClientBase
    {
        /// <summary>Creates a new instance of <see cref="B2Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public B2Client(string keyId, byte[] accessKey, B2Region region, IWebProxy? proxy = null) : this(new B2Config(new AccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="B2Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public B2Client(string keyId, string accessKey, B2Region region, IWebProxy? proxy = null) : this(new B2Config(new StringAccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="B2Client" /></summary>
        /// <param name="credentials">The credentials to use</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public B2Client(IAccessKey credentials, B2Region region, IWebProxy? proxy = null) : this(new B2Config(credentials, region), proxy) { }

        /// <summary>Creates a new instance of <see cref="B2Client" /></summary>
        /// <param name="config">The configuration you want to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public B2Client(B2Config config, IWebProxy? proxy = null) : base(config, proxy) { }

        public B2Client(B2Config options, INetworkDriver networkDriver) : base(options, networkDriver) { }

        public B2Client(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer) : base(objectClient, bucketClient, multipartClient, multipartTransfer, transfer) { }
    }
}