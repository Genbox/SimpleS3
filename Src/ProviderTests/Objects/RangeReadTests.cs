using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class RangeReadTests
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.All, 0, 2, 7)]
    public async Task GetClosedRange(S3Provider _, string bucket, ISimpleClient client, int start) => await ReadRangeAsync(client, bucket, start, false, false);

    [Theory]
    [MultipleProvidersWithData(S3Provider.All, 0, 2, 7)]
    public async Task HeadClosedRange(S3Provider _, string bucket, ISimpleClient client, int start) => await ReadRangeAsync(client, bucket, start, true, false);

    [Theory]
    [MultipleProvidersWithData(S3Provider.All, 0, 2, 7)]
    public async Task SignedGetClosedRange(S3Provider _, string bucket, ISimpleClient client, int start) => await ReadRangeAsync(client, bucket, start, false, true);

    [Theory]
    [MultipleProvidersWithData(S3Provider.All, 0, 2, 7)]
    public async Task SignedHeadClosedRange(S3Provider _, string bucket, ISimpleClient client, int start) => await ReadRangeAsync(client, bucket, start, true, true);

    private static async Task ReadRangeAsync(ISimpleClient client, string bucket, int start, bool head, bool signed)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        string key = "typed-range-" + Guid.NewGuid().ToString("N");
        (int end, string body) = start switch { 0 => (0, "0"), 2 => (7, "234567"), _ => (99, "789") };
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectStringAsync(bucket, key, "0123456789", token: token), async _ =>
        {
            void Configure(HeadObjectRequest request) => request.Range.Add(start, end);
            Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { [HttpHeaders.Range] = $"bytes={start}-{end}" };
            HeadObjectResponse response;

            if (head)
            {
                HeadObjectRequest request = new HeadObjectRequest(bucket, key);
                Configure(request);
                response = signed
                    ? await client.SendSignedRequestAsync<HeadObjectResponse>(client.SignRequest(request, TimeSpan.FromMinutes(2)), HttpMethodType.HEAD, headers: headers, token: token)
                    : await client.HeadObjectAsync(bucket, key, Configure, token);
            }
            else
            {
                GetObjectRequest request = new GetObjectRequest(bucket, key);
                Configure(request);
                using GetObjectResponse get = signed
                    ? await client.SendSignedRequestAsync<GetObjectResponse>(client.SignRequest(request, TimeSpan.FromMinutes(2)), HttpMethodType.GET, headers: headers, token: token)
                    : await client.GetObjectAsync(bucket, key, Configure, token);
                Assert.Equal(206, get.StatusCode);
                Assert.Equal("bytes", get.AcceptRanges);
                Assert.Equal(body, await get.Content.AsStringAsync(token: token));
                Assert.Equal($"bytes {start}-{Math.Min(end, 9)}/10", get.ContentRange);
                Assert.Equal(body.Length, get.ContentLength);
                return;
            }

            Assert.Equal(206, response.StatusCode);
            Assert.Equal("bytes", response.AcceptRanges);
            Assert.Equal($"bytes {start}-{Math.Min(end, 9)}/10", response.ContentRange);
            Assert.Equal(body.Length, response.ContentLength);
        });
    }
}