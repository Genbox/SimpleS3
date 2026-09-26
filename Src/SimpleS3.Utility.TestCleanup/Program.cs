using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Genbox.SimpleS3.Utility.TestCleanup;

internal static class Program
{
    private static async Task<int> Main()
    {
        int failures = 0;

        AnsiConsole.MarkupLine("[bold]Clean up temporary provider test buckets[/]");

        List<S3Provider> providers = await AnsiConsole.PromptAsync(new MultiSelectionPrompt<S3Provider>()
                                                                   .Title("Select the [green]providers[/] to clean up")
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
            IProfileManager profileManager = provider.GetRequiredService<IProfileManager>();

            if (profileManager.GetProfile(profileName) == null)
            {
                AnsiConsole.MarkupLine($"[blue]{Markup.Escape(s3Provider.ToString())}[/]: profile not configured");
                continue;
            }

            ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
            ListBucketsResponse listResponse = await client.ListBucketsAsync(token: CancellationToken.None);

            if (!listResponse.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[blue]{Markup.Escape(s3Provider.ToString())}[/]: [red]unable to list buckets ({listResponse.StatusCode})[/]");
                failures++;
                continue;
            }

            S3Bucket[] buckets = listResponse.Buckets.Where(x => UtilityHelper.IsTemporaryBucket(x.BucketName)).ToArray();

            AnsiConsole.MarkupLine($"[blue]{Markup.Escape(s3Provider.ToString())}[/]: {buckets.Length} temporary bucket(s)");

            foreach (S3Bucket bucket in buckets)
            {
                string bucketName = bucket.BucketName;

                (Exception? failure, bool deferred) = await UtilityHelper.TryDeleteTemporaryBucketAsync(s3Provider, client, bucketName, token: CancellationToken.None);
                string outcome = "[green]deleted[/]";

                if (failure != null)
                {
                    string color = deferred ? "yellow" : "red";
                    string status = deferred ? "deferred" : "failed";
                    outcome = $"[{color}]{status}: {Markup.Escape(failure.Message)}[/]";
                }

                AnsiConsole.MarkupLine($"- {Markup.Escape(bucketName)}: {outcome}");

                if (failure != null)
                    failures++;
            }
        }

        return failures == 0 ? 0 : 1;
    }
}