using System.IO.Enumeration;
using Genbox.SimpleS3.Cli.Core.Abstracts;
using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Exceptions;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Cli.Core.Results;
using Genbox.SimpleS3.Cli.Core.Structs;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Cli.Core.Managers;

public class ObjectManager
{
    private readonly ISimpleClient _client;
    private readonly IFileProvider _fileProvider;

    public ObjectManager(ISimpleClient client, IFileProvider fileProvider)
    {
        _client = client;
        _fileProvider = fileProvider;
    }

    public async IAsyncEnumerable<CopyResult> CopyAsync(string source, string destination)
    {
        if (!ResourceHelper.TryParsePath(source, out PathData src))
            throw new CommandException(ErrorType.Argument, CliErrorMessages.InvalidPath, source);

        if (!ResourceHelper.TryParsePath(destination, out PathData dst))
            throw new CommandException(ErrorType.Argument, CliErrorMessages.InvalidPath, destination);

        IAsyncEnumerable<OutPathData> files = _fileProvider.GetFiles(_client, src, dst);

        if (src.LocationType == LocationType.Local && dst.LocationType == LocationType.Remote)
        {
            await foreach (OutPathData data in files)
            {
                PutObjectResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.PutObjectFileAsync(dst.Bucket, data.RelativeIdentifier, data.FullPath)).ConfigureAwait(false);
                yield return new CopyResult(ActionType.Copy, resp.IsSuccess ? OperationStatus.Success : OperationStatus.Failure, data.FullPath, "S3://" + RemotePathHelper.Combine(dst.Bucket, data.RelativeIdentifier));
            }
        }
        else if (src.LocationType == LocationType.Remote && dst.LocationType == LocationType.Local)
        {
            await foreach (OutPathData data in files)
            {
                GetObjectResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.GetObjectAsync(src.Bucket, data.FullPath)).ConfigureAwait(false);

                if (resp.IsSuccess)
                    await resp.Content.CopyToFileAsync(data.RelativeIdentifier);

                yield return new CopyResult(ActionType.Copy, resp.IsSuccess ? OperationStatus.Success : OperationStatus.Failure, "S3://" + RemotePathHelper.Combine(src.Bucket, data.FullPath), data.RelativeIdentifier);
            }
        }
    }

    public async IAsyncEnumerable<MoveResult> MoveAsync(string source, string destination)
    {
        Dictionary<string, MoveResult> results = new Dictionary<string, MoveResult>(StringComparer.Ordinal);

        await foreach (CopyResult action in CopyAsync(source, destination))
            results.Add(action.Source, new MoveResult(ActionType.Move, action.OperationStatus, action.Source, action.Destination));

        await foreach (DeleteResult action in DeleteAsync(source, false, false))
        {
            MoveResult data = results[action.ObjectKey];
            data.OperationStatus = action.OperationStatus;
            results[action.ObjectKey] = data;
        }

        //TODO: We can yield already in the foreach above
        foreach (KeyValuePair<string, MoveResult> result in results)
            yield return result.Value;
    }

    public async IAsyncEnumerable<DeleteResult> DeleteAsync(string path, bool includeVersions, bool force)
    {
        if (!ResourceHelper.TryParsePath(path, out PathData parsed))
            throw new CommandException(ErrorType.Argument, CliErrorMessages.InvalidPath, path);

        if (parsed.LocationType == LocationType.Local)
        {
            switch (parsed.ResourceType)
            {
                case ResourceType.File:
                {
                    bool success = FileHelper.TryDeleteFile(parsed.Resource);
                    yield return new DeleteResult(ActionType.Delete, success ? OperationStatus.Success : OperationStatus.Failure, parsed.Resource);
                    break;
                }
                case ResourceType.Directory:
                {
                    await foreach (string file in FileHelper.GetFilesAsync(parsed.Resource))
                    {
                        bool success = FileHelper.TryDeleteFile(file);
                        yield return new DeleteResult(ActionType.Delete, success ? OperationStatus.Success : OperationStatus.Failure, file);
                    }
                }
                    break;
                default:
                    throw new CommandException(ErrorType.Argument, CliErrorMessages.OperationNotSupported, path);
            }
        }
        else if (parsed.LocationType == LocationType.Remote)
        {
            switch (parsed.ResourceType)
            {
                case ResourceType.File:
                    DeleteObjectResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.DeleteObjectAsync(parsed.Bucket, parsed.Resource)).ConfigureAwait(false);
                    yield return new DeleteResult(ActionType.Delete, resp.IsSuccess ? OperationStatus.Success : OperationStatus.Failure, "S3://" + RemotePathHelper.Combine(parsed.Bucket, parsed.Resource));
                    break;
                case ResourceType.Directory:
                {
                    Dictionary<string, DeleteResult> results = new Dictionary<string, DeleteResult>(StringComparer.Ordinal);

                    if (includeVersions)
                    {

                        // foreach (S3ObjectVersion version in y.Versions)
                        //     results.Add(version.ObjectKey, new DeleteResult(ActionType.Delete, OperationStatus.Success, version.ObjectKey));
                        //
                        // await RequestHelper.ExecuteAsyncEnumerable(_client, c => c.DeleteAllObjectVersionsAsync(parsed.Bucket));

                        // foreach (S3DeleteError error in z.Errors)
                        // {
                        //     //TODO: Use CollectionsMarshal.GetValueRef when upgrading to .NET 6
                        //     DeleteResult data = results[error.ObjectKey];
                        //     data.OperationStatus = OperationStatus.Failure;
                        //     results[error.ObjectKey] = data;
                        // }
                    }
                    else
                    {
                        // await RequestHelper.ExecuteRequestAsync(_client, c => c.DeleteAllObjectsAsync(parsed.Bucket, null, x => x.Quiet = false, y =>
                        // {
                        //     foreach (S3Object obj in y.Objects)
                        //         results.Add(obj.ObjectKey, new DeleteResult(ActionType.Delete, OperationStatus.Success, obj.ObjectKey));
                        // }, z =>
                        // {
                        //     foreach (S3DeleteError error in z.Errors)
                        //     {
                        //         //TODO: Use CollectionsMarshal.GetValueRef when upgrading to .NET 6
                        //         DeleteResult data = results[error.ObjectKey];
                        //         data.OperationStatus = OperationStatus.Failure;
                        //         results[error.ObjectKey] = data;
                        //     }
                        // })).ConfigureAwait(false);
                    }

                    break;
                }
                default:
                    throw new CommandException(ErrorType.Argument, CliErrorMessages.OperationNotSupported, path);
            }
        }
    }

    public IAsyncEnumerable<S3Object> ListAsync(string path, bool includeOwner)
    {
        if (!ResourceHelper.TryParsePath(path, out PathData parsed))
            throw new CommandException(ErrorType.Argument, CliErrorMessages.InvalidPath, path);

        if (parsed.LocationType != LocationType.Remote)
            throw new CommandException(ErrorType.Argument, CliErrorMessages.S3SyntaxRequired, path);

        return RequestHelper.ExecuteAsyncEnumerable(_client, c => c.ListAllObjectsAsync(parsed.Bucket, req =>
        {
            if (includeOwner)
                req.FetchOwner = true;

            if (parsed.Resource != string.Empty)
                req.Prefix = parsed.Resource;
        }));
    }

    public IAsyncEnumerable<S3Version> ListVersionsAsync(string path)
    {
        if (!ResourceHelper.TryParsePath(path, out PathData parsed))
            throw new CommandException(ErrorType.Argument, CliErrorMessages.InvalidPath, path);

        if (parsed.LocationType != LocationType.Remote)
            throw new CommandException(ErrorType.Argument, CliErrorMessages.S3SyntaxRequired, path);

        return RequestHelper.ExecuteAsyncEnumerable(_client, c => c.ListAllObjectVersionsAsync(path, req =>
        {
            if (parsed.Resource != string.Empty)
                req.Prefix = parsed.Resource;
        }));
    }

    public async Task SyncAsync(string source, string destination, int concurrentUploads, bool preserveTimestamps)
    {
        if (!ResourceHelper.TryParsePath(source, out PathData src))
            throw new CommandException(ErrorType.Argument, CliErrorMessages.InvalidPath, source);

        if (!ResourceHelper.TryParsePath(destination, out PathData dst))
            throw new CommandException(ErrorType.Argument, CliErrorMessages.InvalidPath, destination);

        if (src.ResourceType != ResourceType.Directory)
            throw new CommandException(ErrorType.Argument, CliErrorMessages.ArgumentMustBeDirectory, source);

        if (dst.ResourceType != ResourceType.Directory)
            throw new CommandException(ErrorType.Argument, CliErrorMessages.ArgumentMustBeDirectory, destination);

        //Generate file list of source
        List<FileDateInfo> sourceList;

        if (src.LocationType == LocationType.Local)
            sourceList = GetLocal(src.Bucket, src.Resource).ToList();
        else if (src.LocationType == LocationType.Remote)
            sourceList = await GetRemote(src.Bucket, src.Resource).ToListAsync();
        else
            throw new CommandException(ErrorType.Error, CliErrorMessages.ArgumentOutOfRange, "locationType");

        //Generate file list of destination. It is a dictionary for fast lookup.
        Dictionary<string, FileDateInfo> destinationList;

        if (dst.LocationType == LocationType.Local)
            destinationList = GetLocal(dst.Bucket, dst.Resource).ToDictionary(x => x.ComparisonKey, x => x);
        else if (dst.LocationType == LocationType.Remote)
            destinationList = await GetRemote(dst.Bucket, dst.Resource).ToDictionaryAsync(x => x.ComparisonKey, x => x);
        else
            throw new CommandException(ErrorType.Error, CliErrorMessages.ArgumentOutOfRange, "locationType");

        //We use 3 lists instead of 1 with a state for each file to avoid having to do filter operations on it later.
        //We also use index references here to avoid having to copy the structs.
        List<int> newFiles = new List<int>();
        List<int> modifiedFiles = new List<int>();

        //We use source as the authority
        for (int i = 0; i < sourceList.Count; i++)
        {
            FileDateInfo srcFile = sourceList[i];
            string srcKey = srcFile.ComparisonKey;

            if (destinationList.TryGetValue(srcKey, out FileDateInfo dstFile))
            {
                //The destination had the source file. Determine if it is modified.
                if (srcFile.LastModified > dstFile.LastModified)
                    modifiedFiles.Add(i);

                destinationList.Remove(srcKey);
            }
            else
            {
                //The file does not exist in destination
                newFiles.Add(i);
            }
        }

        //What is left in destinationList at this point are files that are not in source.
        //We remove files first as to not use extra disk space by uploading new files first.

        if (dst.LocationType == LocationType.Local)
        {
            //delete
            if (destinationList.Count > 0)
            {
                foreach (KeyValuePair<string, FileDateInfo> info in destinationList)
                    File.Delete(info.Value.Filename);
            }

            //modified
            if (modifiedFiles.Count > 0)
            {
                await ParallelHelper.ExecuteAsync(modifiedFiles, async (i, token) =>
                {
                    GetObjectResponse resp = await _client.GetObjectAsync(src.Bucket, RemotePathHelper.Combine(src.Resource, sourceList[i].ComparisonKey), token: token);
                    FileInfo info = new FileInfo(LocalPathHelper.Combine(dst.Bucket, dst.Resource, sourceList[i].ComparisonKey));

                    await using (FileStream fs = info.OpenWrite())
                        await resp.Content.CopyToAsync(fs, token);

                    if (preserveTimestamps)
                        info.LastWriteTime = resp.LastModified!.Value.UtcDateTime;
                }, concurrentUploads);
            }

            //new
            if (newFiles.Count > 0)
            {
                await ParallelHelper.ExecuteAsync(newFiles, async (i, token) =>
                {
                    GetObjectResponse resp = await _client.GetObjectAsync(src.Bucket, RemotePathHelper.Combine(src.Resource, sourceList[i].ComparisonKey), token: token);
                    await resp.Content.CopyToFileAsync(LocalPathHelper.Combine(dst.Bucket, dst.Resource, sourceList[i].ComparisonKey));
                }, concurrentUploads);
            }
        }
        else if (dst.LocationType == LocationType.Remote)
        {
            //delete
            if (destinationList.Count > 0)
                await _client.DeleteObjectsAsync(dst.Bucket, destinationList.Select(x => RemotePathHelper.Combine(dst.Resource, x.Value.ComparisonKey)));

            //modified
            if (modifiedFiles.Count > 0)
                await ParallelHelper.ExecuteAsync(modifiedFiles, (i, token) => _client.PutObjectFileAsync(dst.Bucket, RemotePathHelper.Combine(dst.Resource, sourceList[i].ComparisonKey), sourceList[i].Filename, token: token), concurrentUploads);

            //new
            if (newFiles.Count > 0)
                await ParallelHelper.ExecuteAsync(newFiles, (i, token) => _client.PutObjectFileAsync(dst.Bucket, RemotePathHelper.Combine(dst.Resource, sourceList[i].ComparisonKey), sourceList[i].Filename, token: token), concurrentUploads);
        }
    }

    private async IAsyncEnumerable<FileDateInfo> GetRemote(string bucket, string resource)
    {
        await foreach (S3Object obj in _client.ListAllObjectsAsync(bucket, req => req.Prefix = resource))
            yield return new FileDateInfo(obj.ObjectKey.Remove(0, resource.Length), obj.ObjectKey, obj.LastModifiedOn);
    }

    private IEnumerable<FileDateInfo> GetLocal(string bucket, string resource)
    {
        string fullPath = LocalPathHelper.Combine(bucket, resource);

        FileSystemEnumerable<FileDateInfo> enu = new FileSystemEnumerable<FileDateInfo>(fullPath, (ref FileSystemEntry entry) =>
        {
            string path = entry.ToSpecifiedFullPath();
            return new FileDateInfo(path.Remove(0, fullPath.Length), path, entry.LastWriteTimeUtc);
        }, new EnumerationOptions
        {
            AttributesToSkip = FileAttributes.Offline | FileAttributes.ReparsePoint,
            IgnoreInaccessible = true,
            RecurseSubdirectories = true
        })
        {
            ShouldIncludePredicate = (ref FileSystemEntry entry) => !entry.IsDirectory
        };

        return enu;
    }

    private readonly struct FileDateInfo
    {
        public FileDateInfo(string comparisonKey, string filename, DateTimeOffset lastModified)
        {
            ComparisonKey = comparisonKey;
            Filename = filename;
            LastModified = lastModified;
        }

        public string ComparisonKey { get; }

        public string Filename { get; }

        public DateTimeOffset LastModified { get; }
    }
}