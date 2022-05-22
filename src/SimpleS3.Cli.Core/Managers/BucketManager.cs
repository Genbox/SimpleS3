using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Cli.Core.Managers;

public class BucketManager
{
    private readonly ISimpleClient _client;

    public BucketManager(ISimpleClient client)
    {
        _client = client;
    }

    public Task CreateAsync(string bucketName, bool enableLocking, BucketCannedAcl cannedAcl)
    {
        Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

        return RequestHelper.ExecuteRequestAsync(_client, c => c.CreateBucketAsync(bucketName, r =>
        {
            r.EnableObjectLocking = enableLocking;
            r.Acl = cannedAcl;
        }));
    }

    public async IAsyncEnumerable<S3DeleteError> EmptyAsync(string bucketName, bool force)
    {
        Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

        Dictionary<string, S3DeleteError> errors = new Dictionary<string, S3DeleteError>(StringComparer.OrdinalIgnoreCase);

        await foreach (S3DeleteError error in _client.DeleteAllObjectVersionsAsync(bucketName))
            errors.Add(error.ObjectKey, error);

        //If we had errors, it might be because there is a legal hold on the object. User specified force, so we delete it even with the lock
        if (force && errors.Count > 0)
        {
            foreach ((string? key, S3DeleteError? error) in errors)
            {
                PutObjectLegalHoldResponse legalResp = await _client.PutObjectLegalHoldAsync(bucketName, error.ObjectKey, false, r => r.VersionId = error.VersionId);

                if (legalResp.IsSuccess)
                    errors.Remove(key);
            }
        }

        //If there still are errors, it might be because the provider counts incomplete multipart uploads as objects
        if (force && errors.Count > 0)
        {
            //Abort all incomplete multipart uploads
            IAsyncEnumerable<S3Upload> partUploads = _client.ListAllMultipartUploadsAsync(bucketName);

            await foreach (S3Upload partUpload in partUploads)
            {
                AbortMultipartUploadResponse abortResp = await _client.AbortMultipartUploadAsync(bucketName, partUpload.ObjectKey, partUpload.UploadId);

                if (abortResp.IsSuccess)
                    errors.Remove(partUpload.ObjectKey);
            }
        }

        foreach (KeyValuePair<string, S3DeleteError> error in errors)
            yield return error.Value;
    }

    public async IAsyncEnumerable<S3DeleteError> DeleteAsync(string bucketName, bool force)
    {
        Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

        List<S3DeleteError> errors = new List<S3DeleteError>();

        if (force)
        {
            await foreach (S3DeleteError s3DeleteError in EmptyAsync(bucketName, force))
                errors.Add(s3DeleteError);
        }

        if (errors.Count > 0)
        {
            foreach (S3DeleteError error in errors)
                yield return error;
        }
        else
            await RequestHelper.ExecuteRequestAsync(_client, c => c.DeleteBucketAsync(bucketName)).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<S3Bucket> ListAsync([EnumeratorCancellation]CancellationToken token)
    {
        ListBucketsResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.ListBucketsAsync(null, token)).ConfigureAwait(false);

        foreach (S3Bucket respBucket in resp.Buckets)
            yield return respBucket;
    }
}