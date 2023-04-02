using System.ComponentModel.DataAnnotations;
using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Results;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

[Command("rm", Description = "Deletes one or more objects")]
internal class RemoveCommand : OnlineCommandBase
{
    [Argument(0, Description = "The object you want to delete. E.g. s3://bucket/object or s3://bucket/prefix/ to delete a whole prefix")]
    [Required]
    public string Resource { get; set; } = null!;

    [Option("-i|--include-versions", Description = "Also remove all versions of objects")]
    public bool IncludeVersions { get; set; }

    [Option("-f|--force", Description = "Force removal of resources")]
    public bool Force { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        await foreach (DeleteResult result in ObjectManager.DeleteAsync(Resource, IncludeVersions, Force).WithCancellation(token))
        {
            if (result.OperationStatus == OperationStatus.Success)
                Console.WriteLine($"Successfully deleted {result.ObjectKey}");
            else
                Console.WriteLine($"Failed to delete {result.ObjectKey}");
        }
    }
}