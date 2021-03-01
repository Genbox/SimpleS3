using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online.Objects
{
    public class RestoreObjectTests : AwsTestBase
    {
        public RestoreObjectTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task Restore()
        {
            //Upload an object to glacier
            PutObjectResponse putResp = await UploadAsync(nameof(Restore), req => req.StorageClass = StorageClass.Glacier).ConfigureAwait(false);
            Assert.Equal(StorageClass.Glacier, putResp.StorageClass);

            RestoreObjectResponse restoreResp = await ObjectClient.RestoreObjectAsync(BucketName, nameof(Restore), req => req.Days = 2).ConfigureAwait(false);

            Assert.Equal(202, restoreResp.StatusCode);
        }

        [Fact(Skip = "Amazon Glacier retrievals are too often offline for this test to pass.")]
        public async Task RestoreWithSelect()
        {
            using (StringWriter sw = new StringWriter())
            {
                sw.WriteLine("name,age,status");
                sw.WriteLine("santa,800,missing");
                sw.WriteLine("\"donald trump\",7,present");
                sw.WriteLine("fantastic fox,31,missing");

                await UploadAsync(BucketName, nameof(RestoreWithSelect), sw.ToString(), req => req.StorageClass = StorageClass.Glacier).ConfigureAwait(false);
            }

            RestoreObjectResponse restoreResp = await ObjectClient.RestoreObjectAsync(BucketName, nameof(RestoreWithSelect), req =>
            {
                req.RequestType = RestoreRequestType.Select;
                req.Description = "This is a description";
                req.RequestTier = RetrievalTier.Standard;

                S3CsvInputFormat inputFormat = new S3CsvInputFormat();
                inputFormat.HeaderUsage = HeaderUsage.Use;

                S3CsvOutputFormat outputFormat = new S3CsvOutputFormat();
                req.SelectParameters = new S3SelectParameters("SELECT * FROM object WHERE age > 7", inputFormat, outputFormat);

                req.OutputLocation = new S3OutputLocation(BucketName, "outputJob");
                req.OutputLocation.StorageClass = StorageClass.Standard;
            }).ConfigureAwait(false);

            Assert.Equal(202, restoreResp.StatusCode);

            //TODO: List objects beneath outputJob/* and GET file to determine if format is correct
        }
    }
}