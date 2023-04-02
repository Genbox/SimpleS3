using System.Reflection;
using Genbox.SimpleS3.Cli.Commands.Buckets;
using Genbox.SimpleS3.Cli.Commands.Profiles;
using McMaster.Extensions.CommandLineUtils;
using Object = Genbox.SimpleS3.Cli.Commands.Objects.Object;

namespace Genbox.SimpleS3.Cli.Commands;

[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[Command("s3cli")]
[Subcommand(typeof(Bucket), typeof(Profile), typeof(Object))]
public sealed class S3Cli : CommandBase
{
    [Option("-p <profile>", Description = "Set the profile to use")]
    public string? ProfileName { get; set; }

    [Option("-e <endpoint>", Description = "Override the endpoint from the profile")]
    public string? Endpoint { get; set; }

    [Option("--proxy <proxy>", Description = "Use this proxy for request")]
    public string? ProxyUrl { get; set; }

    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        // this shows help even if the --help option isn't specified
        app.ShowHelp();
        return Task.CompletedTask;
    }

    private static string GetVersion()
    {
        AssemblyInformationalVersionAttribute? attr = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        if (attr != null)
            return attr.InformationalVersion;

        return "unknown";
    }
}