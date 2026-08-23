using System.Runtime.CompilerServices;
using System.Web;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Extensions;

public static class MultipartClientExtensions
{
    /// <summary>List all multipart uploads</summary>
    public static async IAsyncEnumerable<S3Upload> ListAllMultipartUploadsAsync(this IMultipartClient client, string bucketName, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);

        string? keyMarker = null;
        string? uploadIdMarker = null;
        ListMultipartUploadsResponse response;

        do
        {
            token.ThrowIfCancellationRequested();

            string? currentKeyMarker = keyMarker;
            string? currentUploadIdMarker = uploadIdMarker;
            response = await client.ListMultipartUploadsAsync(bucketName, req =>
            {
                // DPR-036: S3 multipart pagination requires both key-marker and upload-id-marker.
                req.KeyMarker = currentKeyMarker;
                req.UploadIdMarker = currentUploadIdMarker;
            }, token).ConfigureAwait(false);

            // DPR-036: Do not silently enumerate uploads from failed pages.
            if (!response.IsSuccess)
                throw new S3RequestException(response, $"Unable to list multipart uploads in bucket '{bucketName}");

            foreach (S3Upload responseObject in response.Uploads)
            {
                token.ThrowIfCancellationRequested();
                yield return responseObject;
            }

            keyMarker = response.NextKeyMarker;

            if (response.EncodingType == EncodingType.Url)
                keyMarker = HttpUtility.UrlDecode(keyMarker);

            uploadIdMarker = response.NextUploadIdMarker;
        } while (response.IsTruncated);
    }
}