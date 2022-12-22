using System.Globalization;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Profiles;

[Command(Description = "List all profiles")]
internal class ListCommand : CommandBase
{
    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        Console.WriteLine("{0,-21}{1}", "Created on", "Name");

        foreach (IProfile profile in ServiceManager.ProfileManager.List())
            Console.WriteLine("{0,-21}{1}", profile.CreatedOn.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), profile.Name);

        return Task.CompletedTask;
    }
}