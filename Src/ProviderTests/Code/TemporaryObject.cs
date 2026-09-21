using System.Runtime.ExceptionServices;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.ProviderTests.Code;

// Only use with a unique key owned by this test. Overwrite, retention and
// multi-version scenarios keep their specialized cleanup at the call site.
internal static class TemporaryObject
{
    internal static async Task WithAsync(IObjectClient client, string bucket, string key, Func<Task<PutObjectResponse>> upload, Func<PutObjectResponse, Task> action)
    {
        PutObjectResponse uploaded = await upload();
        Exception? failure = null;

        try
        {
            Assert.Equal(200, uploaded.StatusCode);
            await action(uploaded);
        }
        catch (Exception ex)
        {
            failure = ex;
        }

        if (uploaded.IsSuccess)
        {
            try
            {
                using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                DeleteObjectResponse deleted = await DeleteVersionAsync(uploaded.VersionId, cleanup.Token);

                if (deleted.IsDeleteMarker && deleted.VersionId != uploaded.VersionId)
                {
                    Assert.NotNull(deleted.VersionId);
                    await DeleteVersionAsync(deleted.VersionId, cleanup.Token);
                }

                HeadObjectResponse head = await client.HeadObjectAsync(bucket, key, token: cleanup.Token);

                if (head.IsDeleteMarker)
                {
                    Assert.NotNull(head.VersionId);
                    await DeleteVersionAsync(head.VersionId, cleanup.Token);
                    head = await client.HeadObjectAsync(bucket, key, token: cleanup.Token);
                }

                Assert.Equal(404, head.StatusCode);
                Assert.False(head.IsDeleteMarker);
            }
            catch (Exception ex)
            {
                TestContext.Current.TestOutputHelper?.WriteLine($"Temporary object cleanup failed for '{bucket}/{key}' (version '{uploaded.VersionId}'): {ex}");
                failure ??= ex;
            }
        }

        if (failure != null)
            ExceptionDispatchInfo.Capture(failure).Throw();

        async Task<DeleteObjectResponse> DeleteVersionAsync(string? versionId, CancellationToken token)
        {
            DeleteObjectResponse response = await client.DeleteObjectAsync(bucket, key, request => request.VersionId = versionId, token);
            Assert.Equal(204, response.StatusCode);
            return response;
        }
    }
}