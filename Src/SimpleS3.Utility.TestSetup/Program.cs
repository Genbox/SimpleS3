using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Genbox.SimpleS3.Utility.TestSetup;

internal static class Program
{
    private static async Task Main()
    {
        AnsiConsole.MarkupLine("[bold]Set up the provider test environment[/]");

        List<S3Provider> providers = await AnsiConsole.PromptAsync(new MultiSelectionPrompt<S3Provider>()
                                                                   .Title("Select the [green]providers[/] to configure")
                                                                   .Required()
                                                                   .PageSize(10)
                                                                   .MoreChoicesText("[grey](Move up and down to reveal more providers)[/]")
                                                                   .InstructionsText("[grey](Press [blue]<space>[/] to select, [green]<enter>[/] to accept)[/]")
                                                                   .AddChoices(Enum.GetValues<S3Provider>().Where(x => x is not S3Provider.None and not S3Provider.All)),
            CancellationToken.None);

        foreach (S3Provider s3Provider in providers)
        {
            AnsiConsole.WriteLine();

            string profileName = UtilityHelper.GetProfileName(s3Provider);

            await using ServiceProvider provider = UtilityHelper.CreateSimpleS3(s3Provider, profileName, true);
            IProfile profile = UtilityHelper.GetOrSetupProfile(provider, s3Provider, profileName);

            IBucketClient bucketClient = provider.GetRequiredService<IBucketClient>();

            string bucketName = UtilityHelper.GetTestBucket(profile);
            AnsiConsole.MarkupLine($"Setting up [yellow]{Markup.Escape(bucketName)}[/] in [blue]{Markup.Escape(s3Provider.ToString())}[/]");

            if (await TryCreateBucketAsync(bucketClient, bucketName))
            {
                await SetVersioningAsync(bucketClient, bucketName);

                if (s3Provider == S3Provider.AmazonS3)
                    await PostConfigureAmazonS3Async(bucketClient, bucketName);
                else if (s3Provider == S3Provider.BackBlazeB2)
                    await PostConfigureBackBlazeB2Async(bucketClient, bucketName);
            }
        }
    }

    private static Task PostConfigureBackBlazeB2Async(IBucketClient bucketClient, string bucketName) => SetLockConfigAsync(bucketClient, bucketName);

    private static async Task SetVersioningAsync(IBucketClient bucketClient, string bucketName)
    {
        PutBucketVersioningResponse resp = await bucketClient.PutBucketVersioningAsync(bucketName, true, token: CancellationToken.None);
        WriteResult("Versioning", resp.IsSuccess);
    }

    private static async Task PostConfigureAmazonS3Async(IBucketClient bucketClient, string bucketName)
    {
        await SetLockConfigAsync(bucketClient, bucketName);
        await SetEncryptionConfigAsync(bucketClient, bucketName);
        await SetExpireAllAsync(bucketClient, bucketName);
        await SetPublicAccessBlockAsync(bucketClient, bucketName);
    }

    private static async Task<bool> TryCreateBucketAsync(IBucketClient bucketClient, string bucketName)
    {
        HeadBucketResponse headResp = await bucketClient.HeadBucketAsync(bucketName, token: CancellationToken.None);

        //Bucket already exist - we return true to apply post-configuration
        if (headResp.StatusCode == 200)
        {
            WriteResult("Created", true);
            return true;
        }

        CreateBucketResponse resp = await bucketClient.CreateBucketAsync(bucketName, r =>
        {
            r.ObjectOwnership = ObjectOwnership.BucketOwnerPreferred;
            r.EnableObjectLocking = true;
        }, CancellationToken.None);

        if (resp.IsSuccess)
        {
            WriteResult("Created", true);
            return true;
        }

        WriteResult("Created", false);
        return false;
    }

    private static async Task SetLockConfigAsync(IBucketClient bucketClient, string bucketName)
    {
        PutBucketLockConfigurationResponse resp = await bucketClient.PutBucketLockConfigurationAsync(bucketName, true, token: CancellationToken.None);
        WriteResult("Lock configuration", resp.IsSuccess);
    }

    private static async Task SetEncryptionConfigAsync(IBucketClient bucketClient, string bucketName)
    {
        S3ServerSideEncryptionRule rule = new S3ServerSideEncryptionRule(SseAlgorithm.Aes256);
        rule.BlockedEncryptionTypes.Add(BlockedEncryptionType.None);

        PutBucketEncryptionResponse resp = await bucketClient.PutBucketEncryptionAsync(bucketName, [rule], token: CancellationToken.None);
        WriteResult("Encryption configuration", resp.IsSuccess);
    }

    private static async Task SetExpireAllAsync(IBucketClient bucketClient, string bucketName)
    {
        List<S3Rule> rules =
        [
            new S3Rule("ExpireAll", true)
            {
                AbortIncompleteMultipartUploadDays = 1,
                NonCurrentVersionExpirationDays = 1,
                Expiration = new S3Expiration(1),
                Filter = new S3Filter { Prefix = "" }
            }
        ];

        PutBucketLifecycleConfigurationResponse resp = await bucketClient.PutBucketLifecycleConfigurationAsync(bucketName, rules, token: CancellationToken.None);
        WriteResult("Lifecycle configuration", resp.IsSuccess);
    }

    private static async Task SetPublicAccessBlockAsync(IBucketClient bucketClient, string bucketName)
    {
        PutPublicAccessBlockResponse resp = await bucketClient.PutPublicAccessBlockAsync(bucketName, token: CancellationToken.None);
        WriteResult("Public access block", resp.IsSuccess);
    }

    private static void WriteResult(string operation, bool success) =>
        AnsiConsole.MarkupLine($"- {Markup.Escape(operation)}: {(success ? "[green]OK[/]" : "[red]Failed[/]")}");
}