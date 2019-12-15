using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.LiveTests.Objects
{
    public class RestoreObjectTests : LiveTestBase
    {
        public RestoreObjectTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

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

                await ObjectClient.PutObjectStringAsync(BucketName, nameof(RestoreWithSelect), sw.ToString(), Encoding.UTF8, req => req.StorageClass = StorageClass.Glacier).ConfigureAwait(false);
            }

            RestoreObjectResponse restoreResp = await ObjectClient.RestoreObjectAsync(BucketName, nameof(RestoreWithSelect), req =>
            {
                req.RequestType = RestoreRequestType.Select;
                req.Description = "This is a description";
                req.RequestTier = RetrievalTier.Standard;

                req.SelectParameters.ExpressionType = ExpressionType.Sql;
                req.SelectParameters.Expression = "SELECT * FROM object WHERE age > 7";

                req.SelectParameters.InputFormat = new S3CsvInputFormat
                {
                    HeaderUsage = HeaderUsage.Use
                };

                req.SelectParameters.OutputFormat = new S3CsvOutputFormat();

                req.OutputLocation = new S3OutputLocation(BucketName, "outputJob")
                {
                    StorageClass = StorageClass.Standard
                };
            }).ConfigureAwait(false);

            Assert.Equal(202, restoreResp.StatusCode);

            //TODO: List objects beneath outputJob/* and GET file to determine if format is correct
        }
    }
}