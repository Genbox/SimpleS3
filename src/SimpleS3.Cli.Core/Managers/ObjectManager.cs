using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Core.Responses.S3Types;

namespace Genbox.SimpleS3.Cli.Core.Managers
{
    public class ObjectManager
    {
        private readonly IS3Client _client;

        public ObjectManager(IS3Client client)
        {
            _client = client;
        }

        public async Task<ObjectOperationStatus> CopyAsync(string source, string destination)
        {
            if (!ResourceHelper.TryParseResource(source, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) parsedSource))
                return ObjectOperationStatus.Error;

            if (!ResourceHelper.TryParseResource(destination, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) parsedDestination))
                return ObjectOperationStatus.Error;

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

                        return ObjectOperationStatus.Ok;
                    }
                    case ResourceType.Directory:
                    {
                        switch (parsedDestination.resourceType)
                        {
                            //Source: Local directory - Destination: remote directory
                            case ResourceType.Directory:
                                foreach (string file in Directory.GetFiles(parsedSource.resource))
                                {
                                    string directory = LocalPathHelper.GetDirectoryName(file);
                                    string name = LocalPathHelper.GetFileName(file);
                                    string objectKey = RemotePathHelper.Combine(parsedDestination.resource, directory, name);

                                    using (FileStream fs = File.OpenRead(file))
                                        await RequestHelper.ExecuteRequestAsync(_client, c => c.PutObjectAsync(parsedDestination.bucket, objectKey, fs)).ConfigureAwait(false);
                                }

                                return ObjectOperationStatus.Ok;

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

                        using (Stream s = resp.Content.AsStream())
                        {
                            using (FileStream fs = File.OpenWrite(localFile))
                                await s.CopyToAsync(fs).ConfigureAwait(false);
                        }

                        return ObjectOperationStatus.Ok;
                    }
                    case ResourceType.Directory:
                    {
                        switch (parsedDestination.resourceType)
                        {
                            //Source: remote directory - Destination: local directory
                            case ResourceType.Directory:
                                await foreach (S3Object s3Object in RequestHelper.ExecuteAsyncEnumerable(_client, c => c.ListAllObjectsAsync(parsedSource.bucket, config: req => { req.Prefix = parsedSource.resource; })))
                                {
                                    string destFolder = parsedDestination.resource;
                                    string destFile = LocalPathHelper.Combine(destFolder, parsedDestination.resource, s3Object.ObjectKey);

                                    GetObjectResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.GetObjectAsync(parsedSource.bucket, s3Object.ObjectKey)).ConfigureAwait(false);
                                    await resp.Content.CopyToFileAsync(destFile).ConfigureAwait(false);
                                }

                                return ObjectOperationStatus.Ok;
                            //We don't support file or expand on the destination
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return ObjectOperationStatus.Error;
        }

        public async Task<ObjectOperationStatus> MoveAsync(string source, string destination)
        {
            ObjectOperationStatus status = await CopyAsync(source, destination).ConfigureAwait(false);

            if (status == ObjectOperationStatus.Ok)
                await DeleteAsync(source).ConfigureAwait(false);

            return ObjectOperationStatus.Ok;
        }

        public async Task<ObjectOperationStatus> DeleteAsync(string resource)
        {
            if (!ResourceHelper.TryParseResource(resource, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) parsed))
                return ObjectOperationStatus.Error;

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
                        string continuationToken = null;
                        ListObjectsResponse response;

                        do
                        {
                            string cToken = continuationToken;
                            response = await RequestHelper.ExecuteRequestAsync(_client, c => c.ListObjectsAsync(parsed.bucket, req =>
                            {
                                req.ContinuationToken = cToken;
                                req.Prefix = parsed.resource;
                            })).ConfigureAwait(false);

                            if (!response.IsSuccess)
                                return ObjectOperationStatus.Error;

                            await RequestHelper.ExecuteRequestAsync(_client, c => c.DeleteObjectsAsync(parsed.bucket, response.Objects.Select(x => new S3DeleteInfo(x.ObjectKey, null)))).ConfigureAwait(false);

                            continuationToken = response.NextContinuationToken;
                        } while (response.IsTruncated);

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return ObjectOperationStatus.Ok;
        }
    }
}