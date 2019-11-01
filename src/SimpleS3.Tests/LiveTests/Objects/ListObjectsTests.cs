﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class ListObjectsTests : LiveTestBase
    {
        public ListObjectsTests(ITestOutputHelper helper) : base(helper)
        {
        }

        private async Task UploadString(string bucketName, string objName, SemaphoreSlim semaphore)
        {
            try
            {
                await ObjectClient.PutObjectStringAsync(bucketName, objName, string.Empty).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }

        [Fact]
        public async Task ListMoreThanMaxKeys()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                int concurrent = 10;
                int count = 11;
                Queue<Task> tasks = new Queue<Task>(count);

                using (SemaphoreSlim semaphore = new SemaphoreSlim(concurrent))
                {
                    for (int i = 0; i < count; i++)
                    {
                        await semaphore.WaitAsync().ConfigureAwait(false);

                        string tempObjName = "object-" + Guid.NewGuid();
                        tasks.Enqueue(UploadString(bucket, tempObjName, semaphore));
                    }

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }

                ListObjectsResponse gResp = await ObjectClient.ListObjectsAsync(bucket, req => req.MaxKeys = count - 1).ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal(count - 1, gResp.KeyCount);
                Assert.Equal(count - 1, gResp.Objects.Count);
                Assert.NotEmpty(gResp.NextContinuationToken);
                Assert.True(gResp.IsTruncated);

                ListObjectsResponse gResp2 = await ObjectClient.ListObjectsAsync(bucket, req => req.ContinuationToken = gResp.NextContinuationToken).ConfigureAwait(false);
                Assert.True(gResp2.IsSuccess);

                Assert.Equal(1, gResp2.KeyCount);
                Assert.Equal(1, gResp2.Objects.Count);
                Assert.Equal(gResp.NextContinuationToken, gResp2.ContinuationToken);
                Assert.Null(gResp2.NextContinuationToken);
                Assert.False(gResp2.IsTruncated);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListStandard()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, "hello").ConfigureAwait(false);

                ListObjectsResponse gResp = await ObjectClient.ListObjectsAsync(bucket).ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal(bucket, gResp.BucketName);
                Assert.Equal(1, gResp.KeyCount);
                Assert.Equal(1000, gResp.MaxKeys);
                Assert.False(gResp.IsTruncated);

                S3Object obj = gResp.Objects.First();
                Assert.Equal(tempObjName, obj.ObjectKey);
                Assert.Equal("\"5d41402abc4b2a76b9719d911017c592\"", obj.ETag);
                Assert.Equal(StorageClass.Standard, obj.StorageClass);
                Assert.Equal(5, obj.Size);
                Assert.Equal(DateTime.UtcNow, obj.LastModifiedOn.UtcDateTime, TimeSpan.FromSeconds(5));
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListWithDelimiter()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, "hello").ConfigureAwait(false);
                await ObjectClient.PutObjectStringAsync(bucket, tempObjName2, "world!").ConfigureAwait(false);

                ListObjectsResponse gResp = await ObjectClient.ListObjectsAsync(bucket, req => req.Delimiter = "-").ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal("-", gResp.Delimiter);
                Assert.Equal(2, gResp.KeyCount);
                Assert.Equal(2, gResp.CommonPrefixes.Count);
                Assert.Equal("object-", gResp.CommonPrefixes[0]);
                Assert.Equal("something-", gResp.CommonPrefixes[1]);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListWithEncoding()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "!#/()";

                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, string.Empty).ConfigureAwait(false);

                ListObjectsResponse gResp = await ObjectClient.ListObjectsAsync(bucket, req => req.EncodingType = EncodingType.Url).ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal(EncodingType.Url, gResp.EncodingType);

                S3Object obj = Assert.Single(gResp.Objects);

                Assert.Equal("%21%23/%28%29", obj.ObjectKey);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListWithOwner()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, "hello", config: req => req.AclGrantFullControl.AddEmail(TestConstants.TestEmail)).ConfigureAwait(false);

                ListObjectsResponse gResp = await ObjectClient.ListObjectsAsync(bucket, req => req.FetchOwner = true).ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                S3Object obj = gResp.Objects.First();
                Assert.Equal(TestConstants.TestUsername, obj.Owner.Name);
                Assert.Equal(TestConstants.TestUserId, obj.Owner.Id);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListWithPrefix()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, "hello").ConfigureAwait(false);
                await ObjectClient.PutObjectStringAsync(bucket, tempObjName2, "world!").ConfigureAwait(false);

                ListObjectsResponse gResp = await ObjectClient.ListObjectsAsync(bucket, req => req.Prefix = "object").ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal(1, gResp.KeyCount);
                Assert.Equal("object", gResp.Prefix);

                S3Object obj = Assert.Single(gResp.Objects);

                Assert.Equal(tempObjName, obj.ObjectKey);
            }).ConfigureAwait(false);
        }
    }
}