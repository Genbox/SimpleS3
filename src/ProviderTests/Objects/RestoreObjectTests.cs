using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class RestoreObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task Restore(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            //Upload an object to glacier
            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(Restore), null, req => req.StorageClass = StorageClass.Glacier).ConfigureAwait(false);
            Assert.Equal(StorageClass.Glacier, putResp.StorageClass);

            RestoreObjectResponse restoreResp = await client.RestoreObjectAsync(bucketName, nameof(Restore), req => req.Days = 2).ConfigureAwait(false);

            Assert.Equal(202, restoreResp.StatusCode);
        }

        [Theory(Skip = "Amazon Glacier retrievals are too often offline for this test to pass.")]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task RestoreWithSelect(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            await using (StringWriter sw = new StringWriter())
            {
                sw.WriteLine("name,age,status");
                sw.WriteLine("santa,800,missing");
                sw.WriteLine("\"donald trump\",7,present");
                sw.WriteLine("fantastic fox,31,missing");

                await client.PutObjectStringAsync(bucketName, nameof(RestoreWithSelect), sw.ToString(), null, req => req.StorageClass = StorageClass.Glacier).ConfigureAwait(false);
            }

            RestoreObjectResponse restoreResp = await client.RestoreObjectAsync(bucketName, nameof(RestoreWithSelect), req =>
            {
                req.RequestType = RestoreRequestType.Select;
                req.Description = "This is a description";
                req.RequestTier = RetrievalTier.Standard;

                S3CsvInputFormat inputFormat = new S3CsvInputFormat();
                inputFormat.HeaderUsage = HeaderUsage.Use;

                S3CsvOutputFormat outputFormat = new S3CsvOutputFormat();
                req.SelectParameters = new S3SelectParameters("SELECT * FROM object WHERE age > 7", inputFormat, outputFormat);

                req.OutputLocation = new S3OutputLocation(bucketName, "outputJob");
                req.OutputLocation.StorageClass = StorageClass.Standard;
            }).ConfigureAwait(false);

            Assert.Equal(202, restoreResp.StatusCode);

            //TODO: List objects beneath outputJob/* and GET file to determine if format is correct
        }
    }
}