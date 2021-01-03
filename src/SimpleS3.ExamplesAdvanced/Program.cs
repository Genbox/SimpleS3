﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.ExamplesAdvanced
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //
            // Go here and login with your account to create an access key: https://console.aws.amazon.com/iam/home?#/security_credentials
            //

            //In this example we are using Microsoft's Dependency Injection framework

            //If you have a proxy, you can set it here
            IWebProxy? proxy = new WebProxy("http://127.0.0.1:8888");

            using (S3Client client = BuildClient(proxy))
            {
                string bucketName = "simple-s3-test-" + Guid.NewGuid();
                const string objectName = "some-object";

                //First we create the a bucket named "simple-s3-test". It might already be there, so we ignore if the request was not a success
                await client.CreateBucketAsync(bucketName).ConfigureAwait(false);

                Random random = new Random();

                byte[] data = new byte[1024 * 1024 * 64];
                random.NextBytes(data); //Fill the buffer with random data

                byte[] encryptionKey = Encoding.UTF8.GetBytes("This is our secret encryption ke");

                await UploadData(client, bucketName, objectName, data, encryptionKey);

                byte[] downloaded = await DownloadData(client, bucketName, objectName, encryptionKey);

                if (data.SequenceEqual(downloaded))
                    Console.WriteLine("The uploaded and downloaded data are the same");
                else
                    Console.WriteLine("The uploaded and downloaded data are NOT the same");
            }
        }

        private static async Task UploadData(S3Client client, string bucketName, string objectName, byte[] data, byte[] encryptionKey)
        {
            await using (MemoryStream ms = new MemoryStream(data))
            {
                //Upload using 8 concurrent connections and use server-side encryption with our own key.
                await client.MultipartUploadAsync(bucketName, objectName, ms, 1024 * 1024 * 5, 4, request =>
                  {
                      request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                      request.SseCustomerKey = encryptionKey;
                      request.SseCustomerKeyMd5 = MD5.Create().ComputeHash(encryptionKey);
                  });
            }
        }

        private static async Task<byte[]> DownloadData(S3Client client, string bucketName, string objectName, byte[] encryptionKey)
        {
            await using (MemoryStream ms = new MemoryStream())
            {
                //Upload using 8 concurrent connections and use server-side encryption with our own key.
                await client.MultipartDownloadAsync(bucketName, objectName, ms, numParallelParts: 4, config: request =>
                {
                    request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                    request.SseCustomerKey = encryptionKey;
                    request.SseCustomerKeyMd5 = MD5.Create().ComputeHash(encryptionKey);
                });

                return ms.ToArray();
            }
        }

        private static S3Client BuildClient(IWebProxy? proxy = null)
        {
            //Create the dependency injection container
            ServiceCollection services = new ServiceCollection();

            //We use Microsoft.Extensions.Logging to log to the console
            services.AddLogging(x => x.AddConsole());

            //Here we create a core client. It has no network driver at this point.
            ICoreBuilder coreBuilder = services.AddSimpleS3Core();

            //We want to use HttpClientFactory as the HTTP driver
            IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

            //Add a default timeout and retry policy
            httpBuilder.UseDefaultHttpPolicy();

            //If you set a proxy, this is where it gets added
            if (proxy != null)
                httpBuilder.UseProxy(proxy);

            //This adds the S3Client service. This service combines ObjectClient, MultipartClient and BucketClient into a single client. Makes it easier for new people to use the library.
            coreBuilder.UseS3Client();

            //Here we add the profile manager. It is a profile system that persist your credentials to disk in a very secure way.
            coreBuilder.UseProfileManager()
                       .BindConfigToDefaultProfile() //We can either name the profile (so you can have more than one) or use the default one.
                       .UseConsoleSetup() //This adds a service that ask you to setup your profile if it does not exist.
                       .UseDataProtection(); //This adds encryption using Microsoft's Data Protection library.

            //Finally we build the service provider and return the S3Client
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IProfileManager? profileManager = serviceProvider.GetRequiredService<IProfileManager>();
            IProfile? profile = profileManager.GetDefaultProfile();

            if (profile == null)
            {
                ConsoleProfileSetup? setup = serviceProvider.GetRequiredService<ConsoleProfileSetup>();
                setup.SetupDefaultProfile();
            }

            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}