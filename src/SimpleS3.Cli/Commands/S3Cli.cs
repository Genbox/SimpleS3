using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Cli.Commands.Buckets;
using Genbox.SimpleS3.Cli.Commands.Objects;
using McMaster.Extensions.CommandLineUtils;
using ListCommand = Genbox.SimpleS3.Cli.Commands.Objects.ListCommand;

namespace Genbox.SimpleS3.Cli.Commands
{
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Command("s3cli")]
    [Subcommand(typeof(Bucket), typeof(RemoveCommand), typeof(CopyCommand), typeof(MoveCommand), typeof(ListCommand))]
    public sealed class S3Cli : CommandBase<S3Cli>
    {
        [Option("-p <profile>", Description = "Set the profile to use")]
        public string ProfileName { get; set; }

        [Option("-r <region>", Description = "Set the region to use")]
        public AwsRegion Region { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            // this shows help even if the --help option isn't specified
            app.ShowHelp();
        }

        private static string GetVersion()
        {
            return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}