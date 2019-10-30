using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class S3ObjectClientExtensions
    {
        public static async Task<DeleteObjectResponse> DeleteObjectAsync(this IS3ObjectClient client, string bucketName, string objectKey, string versionId = null, MfaAuthenticationBuilder mfa = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            DeleteObjectResponse resp = await client.DeleteObjectAsync(bucketName, objectKey, req =>
            {
                req.VersionId = versionId;
                req.Mfa = mfa;
            }, token).ConfigureAwait(false);

            return resp;
        }

        public static Task<DeleteObjectsResponse> DeleteObjectsAsync(this IS3ObjectClient client, string bucketName, IEnumerable<string> objectKeys, Action<DeleteObjectsRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKeys, nameof(objectKeys));

            return client.DeleteObjectsAsync(bucketName, objectKeys.Select(x => new S3DeleteInfo(x, null)), config, token);
        }

        public static Task<DeleteObjectsResponse> DeleteObjectsAsync(this IS3ObjectClient client, string bucketName, IEnumerable<string> objectKeys, bool quiet = true, MfaAuthenticationBuilder mfa = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKeys, nameof(objectKeys));

            return client.DeleteObjectsAsync(bucketName, objectKeys.Select(x => new S3DeleteInfo(x, null)), req =>
            {
                req.Mfa = mfa;
                req.Quiet = quiet;
            }, token);
        }

        public static Task<DeleteObjectsResponse> DeleteObjectsAsync(this IS3ObjectClient client, string bucketName, IEnumerable<S3DeleteInfo> objectKeys, bool quiet = true, MfaAuthenticationBuilder mfa = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKeys, nameof(objectKeys));

            return client.DeleteObjectsAsync(bucketName, objectKeys, req =>
            {
                req.Mfa = mfa;
                req.Quiet = quiet;
            }, token);
        }

        public static async Task<PutObjectResponse> PutObjectDataAsync(this IS3ObjectClient client, string bucketName, string objectKey, byte[] data, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            using (MemoryStream ms = new MemoryStream(data))
                return await client.PutObjectAsync(bucketName, objectKey, ms, config, token).ConfigureAwait(false);
        }

        public static Task<PutObjectResponse> PutObjectStringAsync(this IS3ObjectClient client, string bucketName, string objectKey, string content, Encoding encoding = null, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            if (encoding == null)
                encoding = Encoding.UTF8;

            return client.PutObjectDataAsync(bucketName, objectKey, encoding.GetBytes(content), config, token);
        }

        public static async Task<PutObjectResponse> PutObjectFileAsync(this IS3ObjectClient client, string bucketName, string objectKey, string file, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            if (!File.Exists(file))
                throw new FileNotFoundException("The file does not exist.", file);

            using (FileStream fs = File.OpenRead(file))
                return await client.PutObjectAsync(bucketName, objectKey, fs, config, token).ConfigureAwait(false);
        }

        public static async Task<ContentReader> GetObjectContentAsync(this IS3ObjectClient client, string bucketName, string objectKey, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, objectKey, config, token).ConfigureAwait(false);

            if (resp.IsSuccess)
                return resp.Content;

            return null;
        }

        public static async Task<string> GetObjectStringAsync(this IS3ObjectClient client, string bucketName, string objectKey, Encoding encoding = null, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            ContentReader content = await GetObjectContentAsync(client, bucketName, objectKey, config, token).ConfigureAwait(false);

            if (content != null)
                return await content.AsStringAsync(encoding).ConfigureAwait(false);

            return null;
        }
    }
}