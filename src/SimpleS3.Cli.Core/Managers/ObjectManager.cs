using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Exceptions;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Cli.Core.Managers
{
    public class ObjectManager
    {
        private readonly IClient _client;

        public ObjectManager(IClient client)
        {
            _client = client;
        }

        public async Task CopyAsync(string source, string destination)
        {
            if (!ResourceHelper.TryParseResource(source, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) parsedSource))
                throw new CommandException(ErrorType.Argument, $"Failed to parse source: {source}");

            if (!ResourceHelper.TryParseResource(destination, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) parsedDestination))
                throw new CommandException(ErrorType.Argument, $"Failed to parse destination: {destination}");

            if (parsedSource.bucket == null)
                throw new S3Exception("Unable to parse bucket in source");

            if (parsedDestination.bucket == null)
                throw new S3Exception("Unable to parse bucket in destination");

            if (parsedSource.locationType == LocationType.Local && parsedDestination.locationType == LocationType.Remote)
            {
                switch (parsedSource.resourceType)
                {
                    case ResourceType.File:
                    {
                        string objectKey;

                        switch (parsedDestination.resourceType)
                        {
                            //Source: Local file - Destination: remote file
                            case ResourceType.File:
                                objectKey = parsedDestination.resource;
                                break;

                            //Source: Local file - Destination: remote directory
                            case ResourceType.Directory:
                                objectKey = RemotePathHelper.Combine(parsedDestination.resource, LocalPathHelper.GetFileName(parsedSource.resource));
                                break;

                            //We don't support expand on the destination
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        using (FileStream fs = File.OpenRead(parsedSource.resource))
                            await RequestHelper.ExecuteRequestAsync(_client, c => c.PutObjectAsync(parsedDestination.bucket, objectKey, fs)).ConfigureAwait(false);

                        return;
                    }
                    case ResourceType.Directory:
                    {
                        switch (parsedDestination.resourceType)
                        {
                            //Source: Local directory - Destination: remote directory
                            case ResourceType.Directory:
                                foreach (string file in Directory.GetFiles(parsedSource.resource))
                                {
                                    string? directory = LocalPathHelper.GetDirectoryName(file);
                                    string name = LocalPathHelper.GetFileName(file);
                                    string objectKey = RemotePathHelper.Combine(parsedDestination.resource, directory, name);

                                    using (FileStream fs = File.OpenRead(file))
                                        await RequestHelper.ExecuteRequestAsync(_client, c => c.PutObjectAsync(parsedDestination.bucket, objectKey, fs)).ConfigureAwait(false);
                                }

                                return;

                            //We don't support files or expand on the destination
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (parsedSource.locationType == LocationType.Remote && parsedDestination.locationType == LocationType.Local)
            {
                switch (parsedSource.resourceType)
                {
                    case ResourceType.File:
                    {
                        string localFile;

                        switch (parsedDestination.resourceType)
                        {
                            //Source: remote file - Destination: local file
                            case ResourceType.File:
                                localFile = parsedDestination.resource;
                                break;

                            //Source: remote file - Destination: local directory
                            case ResourceType.Directory:
                                localFile = LocalPathHelper.Combine(parsedDestination.resource, RemotePathHelper.GetFileName(parsedSource.resource));
                                break;

                            //We don't support expand on the destination
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        GetObjectResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.GetObjectAsync(parsedSource.bucket, parsedSource.resource)).ConfigureAwait(false);

                        using (Stream s = resp.Content)
                        using (FileStream fs = File.OpenWrite(localFile))
                            await s.CopyToAsync(fs).ConfigureAwait(false);

                        return;
                    }
                    case ResourceType.Directory:
                    {
                        switch (parsedDestination.resourceType)
                        {
                            //Source: remote directory - Destination: local directory
                            case ResourceType.Directory:
                                await foreach (S3Object s3Object in RequestHelper.ExecuteAsyncEnumerable(_client, c => c.ListAllObjectsAsync(parsedSource.bucket, config: req =>
                                {
                                    req.Prefix = parsedSource.resource;
                                })))
                                {
                                    string destFolder = parsedDestination.resource;
                                    string destFile = LocalPathHelper.Combine(destFolder, parsedDestination.resource, s3Object.ObjectKey);

                                    GetObjectResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.GetObjectAsync(parsedSource.bucket, s3Object.ObjectKey)).ConfigureAwait(false);
                                    await resp.Content.CopyToFileAsync(destFile).ConfigureAwait(false);
                                }

                                return;

                            //We don't support file or expand on the destination
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public async Task MoveAsync(string source, string destination)
        {
            await CopyAsync(source, destination).ConfigureAwait(false);
            await DeleteAsync(source).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string resource)
        {
            if (!ResourceHelper.TryParseResource(resource, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) parsed))
                throw new CommandException(ErrorType.Argument, $"Failed to parse resource: {resource}");

            if (parsed.bucket == null)
                throw new S3Exception("Unable to parse bucket");

            if (parsed.locationType == LocationType.Local)
            {
                switch (parsed.resourceType)
                {
                    case ResourceType.File:
                        File.Delete(parsed.resource);
                        break;
                    case ResourceType.Directory:
                        Directory.Delete(parsed.resource, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (parsed.resourceType)
                {
                    case ResourceType.File:
                        await RequestHelper.ExecuteRequestAsync(_client, c => c.DeleteObjectAsync(parsed.bucket, parsed.resource)).ConfigureAwait(false);
                        break;
                    case ResourceType.Directory:
                        string? continuationToken = null;
                        ListObjectsResponse response;

                        do
                        {
                            string? cToken = continuationToken;
                            response = await RequestHelper.ExecuteRequestAsync(_client, c => c.ListObjectsAsync(parsed.bucket, req =>
                            {
                                req.ContinuationToken = cToken;
                                req.Prefix = parsed.resource;
                            })).ConfigureAwait(false);

                            if (!response.IsSuccess)
                                throw new CommandException(ErrorType.RequestError, "List request failed");

                            await RequestHelper.ExecuteRequestAsync(_client, c => c.DeleteObjectsAsync(parsed.bucket, response.Objects.Select(x => new S3DeleteInfo(x.ObjectKey)))).ConfigureAwait(false);

                            continuationToken = response.NextContinuationToken;
                        } while (response.IsTruncated);

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public IAsyncEnumerable<S3Object> ListAsync(string bucketName, bool includeOwner)
        {
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            return RequestHelper.ExecuteAsyncEnumerable(_client, c => c.ListAllObjectsAsync(bucketName, includeOwner));
        }
    }
}