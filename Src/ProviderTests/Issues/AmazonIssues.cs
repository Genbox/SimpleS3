using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Issues;

public class AmazonIssues : TestBase
{
    /// <summary>https://github.com/Genbox/SimpleS3/issues/66</summary>
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task Issue66(S3Provider provider, string bucket, ISimpleClient client)
    {
        //Issue: When setting EncodingType to Url, it creates an infinite loop
        //Investigation: Amazon URL encode the NextKeyMarker value, so when returned to AWS as KeyMarker, it gets double encoded.
        //Resolution: KeyMarker is now URL decoded before sent to AWS

        string[] names = ["a%20.txt", "a&1.txt", "a(1).exe", "folder1/a%20.txt"];

        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            //Create objects
            foreach (string name in names)
                await client.PutObjectStringAsync(tempBucket, name, "");

            //List them while forcing pagination
            List<S3Version> objects = await ToListAsync(client.ListAllObjectVersionsAsync(tempBucket, r =>
            {
                r.MaxKeys = 2;
                r.EncodingType = EncodingType.Url;
            }));

            Assert.Equal(4, objects.Count);
        });
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> list)
    {
        List<T> res = new List<T>();

        await foreach (T item in list)
            res.Add(item);

        return res;
    }
}