using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Signed;

public class SignedTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task FullPreSignTest(S3Provider _, string bucket, ISimpleClient client)
    {
        TimeSpan expire = TimeSpan.FromSeconds(100);
        const string objectKey = "test.zip";

        PutObjectRequest putReq = new PutObjectRequest(bucket, objectKey, null);

        string url = client.SignRequest(putReq, expire);

        await using (MemoryStream ms = new MemoryStream("hello world"u8.ToArray()))
        {
            PutObjectResponse putResp = await client.SendSignedRequestAsync<PutObjectResponse>(url, HttpMethodType.PUT, ms);
            Assert.Equal(200, putResp.StatusCode);
        }

        GetObjectRequest getReq = new GetObjectRequest(bucket, objectKey);
        url = client.SignRequest(getReq, expire);

        GetObjectResponse getResp = await client.SendSignedRequestAsync<GetObjectResponse>(url, HttpMethodType.GET);
        Assert.Equal(200, getResp.StatusCode);

        DeleteObjectRequest deleteReq = new DeleteObjectRequest(bucket, objectKey);
        url = client.SignRequest(deleteReq, expire);

        DeleteObjectResponse deleteResp = await client.SendSignedRequestAsync<DeleteObjectResponse>(url, HttpMethodType.DELETE);
        Assert.Equal(204, deleteResp.StatusCode);

        HeadObjectRequest headReq = new HeadObjectRequest(bucket, objectKey);
        url = client.SignRequest(headReq, expire);

        HeadObjectResponse headResp = await client.SendSignedRequestAsync<HeadObjectResponse>(url, HttpMethodType.HEAD);
        Assert.Equal(404, headResp.StatusCode);
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task PreSignedMultipartUpload(S3Provider _, string bucket, ISimpleClient client)
    {
        const string key = "data.txt";
        TimeSpan expire = TimeSpan.FromSeconds(100);

        //Create (pre-signed)
        CreateMultipartUploadRequest createReq = new CreateMultipartUploadRequest(bucket, key);
        string url = client.SignRequest(createReq, expire);
        CreateMultipartUploadResponse createResp = await client.SendSignedRequestAsync<CreateMultipartUploadResponse>(url, HttpMethodType.POST);
        Assert.Equal(200, createResp.StatusCode);

        List<S3PartInfo> infos = new List<S3PartInfo>();

        await using (MemoryStream ms = new MemoryStream("hello world"u8.ToArray()))
        {
            //Upload (pre-signed)
            UploadPartRequest req = new UploadPartRequest(bucket, key, createResp.UploadId, 1, null);
            url = client.SignRequest(req, expire);
            UploadPartResponse partResp = await client.SendSignedRequestAsync<UploadPartResponse>(url, HttpMethodType.PUT, ms);
            Assert.Equal(200, partResp.StatusCode);

            infos.Add(new S3PartInfo(partResp.ETag, 1));
        }

        //Complete (not pre-signed due to using XML)
        CompleteMultipartUploadResponse compResp = await client.CompleteMultipartUploadAsync(bucket, key, createResp.UploadId, infos);
        Assert.Equal(200, compResp.StatusCode);
    }
}