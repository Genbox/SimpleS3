using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class MultipartClientExtensions
    {
        /// <summary>List all multipart uploads</summary>
        public static async IAsyncEnumerable<S3Upload> ListAllMultipartUploadsAsync(this IMultipartClient client, string bucketName, [EnumeratorCancellation]CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));

            string? uploadIdMarker = null;
            ListMultipartUploadsResponse response;

            do
            {
                if (token.IsCancellationRequested)
                    break;

                string? marker = uploadIdMarker;
                response = await client.ListMultipartUploadsAsync(bucketName, req => req.UploadIdMarker = marker, token).ConfigureAwait(false);

                foreach (S3Upload responseObject in response.Uploads)
                {
                    yield return responseObject;
                }

                uploadIdMarker = response.NextUploadIdMarker;
            } while (response.IsTruncated);
        }
    }
}