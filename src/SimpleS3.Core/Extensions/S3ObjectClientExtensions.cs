using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Internal;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Objects;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class S3ObjectClientExtensions
    {
        public static async Task<DeleteObjectResponse> DeleteObjectAsync(this IS3ObjectClient client, string bucketName, string resource, string versionId = null, MfaAuthenticationBuilder mfa = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resource);

            DeleteObjectResponse resp = await client.DeleteObjectAsync(bucketName, resource, req =>
            {
                req.VersionId = versionId;
                req.Mfa = mfa;
            }, token).ConfigureAwait(false);

            return resp;
        }

        public static Task<DeleteMultipleObjectsResponse> DeleteMultipleObjectsAsync(this IS3ObjectClient client, string bucketName, IEnumerable<string> resources, Action<DeleteMultipleObjectsRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resources);

            return client.DeleteMultipleObjectsAsync(bucketName, resources.Select(x => new S3DeleteInfo(x, null)), config, token);
        }

        public static Task<DeleteMultipleObjectsResponse> DeleteMultipleObjectsAsync(this IS3ObjectClient client, string bucketName, IEnumerable<string> resources, MfaAuthenticationBuilder mfa = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resources);

            return client.DeleteMultipleObjectsAsync(bucketName, resources.Select(x => new S3DeleteInfo(x, null)), req => req.Mfa = mfa, token);
        }

        public static Task<DeleteMultipleObjectsResponse> DeleteMultipleObjectsAsync(this IS3ObjectClient client, string bucketName, IEnumerable<S3DeleteInfo> resources, MfaAuthenticationBuilder mfa = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resources);

            return client.DeleteMultipleObjectsAsync(bucketName, resources, req => req.Mfa = mfa, token);
        }

        public static async Task<PutObjectResponse> PutObjectDataAsync(this IS3ObjectClient client, string bucketName, string resource, byte[] data, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resource);

            using (MemoryStream ms = new MemoryStream(data))
                return await client.PutObjectAsync(bucketName, resource, ms, config, token).ConfigureAwait(false);
        }

        public static Task<PutObjectResponse> PutObjectStringAsync(this IS3ObjectClient client, string bucketName, string resource, string content, Encoding encoding = null, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resource);

            if (encoding == null)
                encoding = Encoding.UTF8;

            return client.PutObjectDataAsync(bucketName, resource, encoding.GetBytes(content), config, token);
        }

        public static async Task<PutObjectResponse> PutObjectFileAsync(this IS3ObjectClient client, string bucketName, string resource, string file, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resource);

            if (!File.Exists(file))
                throw new FileNotFoundException("The file does not exist.", file);

            using (FileStream fs = File.OpenRead(file))
                return await client.PutObjectAsync(bucketName, resource, fs, config, token).ConfigureAwait(false);
        }

        public static async Task<ContentReader> GetObjectContentAsync(this IS3ObjectClient client, string bucketName, string resource, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resource);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, resource, config, token).ConfigureAwait(false);

            if (resp.IsSuccess)
                return resp.Content;

            return null;
        }

        public static async Task<string> GetObjectStringAsync(this IS3ObjectClient client, string bucketName, string resource, Encoding encoding = null, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client);
            Validator.RequireNotNull(bucketName);
            Validator.RequireNotNull(resource);

            ContentReader content = await GetObjectContentAsync(client, bucketName, resource, config, token).ConfigureAwait(false);

            if (content != null)
                return await content.AsStringAsync(encoding).ConfigureAwait(false);

            return null;
        }
    }
}