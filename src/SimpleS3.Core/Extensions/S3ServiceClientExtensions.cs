using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Internal;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Core.Responses.Service;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class S3ServiceClientExtensions
    {
        public static async IAsyncEnumerable<S3Bucket> GetServiceAllAsync(this IS3ServiceClient client, Action<GetServiceRequest> config = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(client);

            GetServiceResponse resp = await client.GetServiceAsync(config, token).ConfigureAwait(false);

            foreach (S3Bucket respBucket in resp.Buckets)
                yield return respBucket;
        }
    }
}