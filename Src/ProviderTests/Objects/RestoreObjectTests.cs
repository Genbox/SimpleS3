using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class RestoreObjectTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task Restore(S3Provider _, string bucket, ISimpleClient client)
    {
        //Upload an object to glacier
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(Restore), null, r => r.StorageClass = StorageClass.Glacier);
        Assert.Equal(StorageClass.Glacier, putResp.StorageClass);

        RestoreObjectResponse restoreResp = await client.RestoreObjectAsync(bucket, nameof(Restore), r => r.Days = 2);

        Assert.Equal(202, restoreResp.StatusCode);
    }

    [Theory(Skip = "Amazon Glacier retrievals are too often offline for this test to pass.")]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task RestoreWithSelect(S3Provider _, string bucket, ISimpleClient client)
    {
        await using (StringWriter sw = new StringWriter())
        {
            await sw.WriteLineAsync("name,age,status");
            await sw.WriteLineAsync("santa,800,missing");
            await sw.WriteLineAsync("\"donald trump\",7,present");
            await sw.WriteLineAsync("fantastic fox,31,missing");

            await client.PutObjectStringAsync(bucket, nameof(RestoreWithSelect), sw.ToString(), null, r => r.StorageClass = StorageClass.Glacier);
        }

        RestoreObjectResponse restoreResp = await client.RestoreObjectAsync(bucket, nameof(RestoreWithSelect), r =>
        {
            r.RequestType = RestoreRequestType.Select;
            r.Description = "This is a description";
            r.RequestTier = RetrievalTier.Standard;

            S3CsvInputFormat inputFormat = new S3CsvInputFormat();
            inputFormat.HeaderUsage = HeaderUsage.Use;

            S3CsvOutputFormat outputFormat = new S3CsvOutputFormat();
            r.SelectParameters = new S3SelectParameters("SELECT * FROM object WHERE age > 7", inputFormat, outputFormat);

            r.OutputLocation = new S3OutputLocation(bucket, "outputJob");
            r.OutputLocation.StorageClass = StorageClass.Standard;
        });

        Assert.Equal(202, restoreResp.StatusCode);

        //TODO: List objects beneath outputJob/* and GET file to determine if format is correct
    }
}