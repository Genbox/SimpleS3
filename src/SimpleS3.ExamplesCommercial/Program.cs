using Genbox.SimpleS3.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.ExamplesCommercial;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // This example uses BackBlaze B2. Go here to create a key: https://secure.backblaze.com/app_keys.htm

        ISimpleClient client = BuildClient();

        string bucketName = "test-" + Guid.NewGuid();

        Console.WriteLine("# Listing buckets");

        await foreach (S3Bucket bucket in client.ListAllBucketsAsync()) //This is a commercial feature
            Console.WriteLine($" - {bucket.BucketName}");

        Console.WriteLine("# Creating a temporary bucket");

        CreateBucketResponse putBucket = await client.CreateBucketAsync(bucketName);

        if (putBucket.IsSuccess)
        {
            Console.WriteLine($" - Successfully created {bucketName}");

            Console.WriteLine("# Creating 10 objects");

            for (int i = 0; i < 10; i++)
            {
                string objectName = Guid.NewGuid().ToString();

                PutObjectResponse putReq = await client.PutObjectStringAsync(bucketName, objectName, "This is a test"); //This is a commercial feature

                if (putReq.IsSuccess)
                    Console.WriteLine($" - Successfully created {objectName}");
                else
                    Console.WriteLine($" - Failed to create {objectName}");
            }

            Console.WriteLine("# Deleting all objects in temporary bucket");

            await foreach (S3DeleteError obj in client.DeleteAllObjectVersionsAsync(bucketName)) //This is a commercial feature
                Console.WriteLine("Deleted " + obj.ObjectKey);

            DeleteBucketResponse delResp = await client.DeleteBucketAsync(bucketName);

            if (delResp.IsSuccess)
                Console.WriteLine($" - Successfully deleted {bucketName}");
            else
                Console.WriteLine($" - Failed to delete {bucketName}");
        }
        else
            Console.WriteLine("Failed to create " + bucketName);
    }

    private static ISimpleClient BuildClient()
    {
        ServiceCollection services = new ServiceCollection();

        IClientBuilder builder = services.AddBackBlazeB2(); //This is a commercial feature
        builder.CoreBuilder.UsePooledClients(); //This is a commercial feature

        builder.CoreBuilder.UseProfileManager()
               .BindConfigToDefaultProfile()
               .UseConsoleSetup()
               .UseDataProtection(); //This is a commercial feature

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        IProfileManager profileManager = serviceProvider.GetRequiredService<IProfileManager>();
        IProfile? profile = profileManager.GetDefaultProfile();

        if (profile == null)
        {
            IProfileSetup setup = serviceProvider.GetRequiredService<IProfileSetup>();
            setup.SetupDefaultProfile();
        }

        return serviceProvider.GetRequiredService<ISimpleClient>();
    }
}