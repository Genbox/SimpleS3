using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class ListObjectsTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task ListObjects(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                PutObjectResponse putResp = await client.PutObjectStringAsync(tempBucket, tempObjName, "hello").ConfigureAwait(false);
                Assert.Equal(200, putResp.StatusCode);

                ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucket).ConfigureAwait(false);
                Assert.Equal(200, listResp.StatusCode);

                Assert.Equal(tempBucket, listResp.BucketName);
                Assert.Equal(1, listResp.KeyCount);

                if (provider == S3Provider.AmazonS3)
                    Assert.Equal(1000, listResp.MaxKeys);

                Assert.False(listResp.IsTruncated);

                S3Object obj = listResp.Objects.First();
                Assert.Equal(tempObjName, obj.ObjectKey);
                Assert.Equal("\"5d41402abc4b2a76b9719d911017c592\"", obj.ETag);
                Assert.Equal(StorageClass.Standard, obj.StorageClass);
                Assert.Equal(5, obj.Size);
                Assert.Equal(DateTime.UtcNow, obj.LastModifiedOn.UtcDateTime, TimeSpan.FromSeconds(5));
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task ListObjectsMoreThanMaxKeys(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                int concurrent = 10;
                int count = 11;

                IEnumerable<PutObjectResponse> responses = await ParallelHelper.ExecuteAsync(Enumerable.Range(0, count), i => client.PutObjectAsync(tempBucket, i.ToString(), null), concurrent, CancellationToken.None);

                foreach (PutObjectResponse putResp in responses)
                {
                    Assert.Equal(200, putResp.StatusCode);
                }

                ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucket, r => r.MaxKeys = count - 1).ConfigureAwait(false);
                Assert.Equal(200, listResp.StatusCode);

                Assert.Equal(count - 1, listResp.KeyCount);
                Assert.Equal(count - 1, listResp.Objects.Count);
                Assert.True(listResp.IsTruncated);

                if (provider == S3Provider.AmazonS3)
                    Assert.NotEmpty(listResp.NextContinuationToken);

                ListObjectsResponse listResp2 = await client.ListObjectsAsync(tempBucket, r => r.ContinuationToken = listResp.NextContinuationToken).ConfigureAwait(false);
                Assert.Equal(200, listResp2.StatusCode);

                Assert.Equal(1, listResp2.KeyCount);
                Assert.Equal(1, listResp2.Objects.Count);
                Assert.Equal(listResp.NextContinuationToken, listResp2.ContinuationToken);
                Assert.Null(listResp2.NextContinuationToken);
                Assert.False(listResp2.IsTruncated);
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)] //Google supports delimiter, but it is sorely broken
        public async Task ListObjectsWithDelimiter(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await client.PutObjectAsync(tempBucket, tempObjName, null).ConfigureAwait(false);
                await client.PutObjectAsync(tempBucket, tempObjName2, null).ConfigureAwait(false);

                ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucket, r => r.Delimiter = "-").ConfigureAwait(false);
                Assert.Equal(200, listResp.StatusCode);

                Assert.Equal("-", listResp.Delimiter);
                Assert.Equal(2, listResp.KeyCount);
                Assert.Equal(2, listResp.CommonPrefixes.Count);
                Assert.Equal("object-", listResp.CommonPrefixes[0]);
                Assert.Equal("something-", listResp.CommonPrefixes[1]);
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task ListObjectsWithEncoding(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                string tempObjName = "!#/()";

                PutObjectResponse putResp = await client.PutObjectAsync(tempBucket, tempObjName, null).ConfigureAwait(false);
                Assert.Equal(200, putResp.StatusCode);

                ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucket, r => r.EncodingType = EncodingType.Url).ConfigureAwait(false);
                Assert.Equal(200, listResp.StatusCode);

                Assert.Equal(EncodingType.Url, listResp.EncodingType);

                S3Object? obj = Assert.Single(listResp.Objects);

                Assert.Equal("%21%23/%28%29", obj.ObjectKey);
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
        public async Task ListObjectsWithOwner(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                PutObjectResponse putResp = await client.PutObjectAsync(tempBucket, tempObjName, null, r => r.AclGrantFullControl.AddEmail(TestConstants.TestEmail)).ConfigureAwait(false);
                Assert.Equal(200, putResp.StatusCode);

                ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucket, req => req.FetchOwner = true).ConfigureAwait(false);
                Assert.Equal(200, listResp.StatusCode);

                S3Object obj = listResp.Objects.First();

                if (provider == S3Provider.AmazonS3)
                {
                    Assert.Equal(TestConstants.TestUsername, obj.Owner!.Name);
                    Assert.Equal(TestConstants.TestUserId, obj.Owner.Id);
                }
                else
                    Assert.NotNull(obj.Owner);

            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task ListObjectsWithPrefix(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                PutObjectResponse putResp1 = await client.PutObjectAsync(tempBucket, tempObjName, null).ConfigureAwait(false);
                Assert.Equal(200, putResp1.StatusCode);

                PutObjectResponse putResp2 = await client.PutObjectAsync(tempBucket, tempObjName2, null).ConfigureAwait(false);
                Assert.Equal(200, putResp2.StatusCode);

                ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucket, req => req.Prefix = "object").ConfigureAwait(false);
                Assert.Equal(200, listResp.StatusCode);

                Assert.Equal(1, listResp.KeyCount);
                Assert.Equal("object", listResp.Prefix);

                S3Object? obj = Assert.Single(listResp.Objects);

                Assert.Equal(tempObjName, obj.ObjectKey);
            }).ConfigureAwait(false);
        }
    }
}