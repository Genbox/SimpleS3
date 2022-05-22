using System.ComponentModel.DataAnnotations;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Profile;

[Command(Description = "Create a new profile")]
internal class CreateCommand : CommandBase
{
    [Argument(0)]
    [Required]
    public string ProfileName { get; set; } = null!;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        IProfile? profile = ServiceManager.ProfileManager.GetProfile(ProfileName);

        if (profile != null)
            Console.WriteLine($"The profile '{ProfileName}' already exist.");
        else
        {
            profile = ServiceManager.ConsoleSetup.SetupProfile(ProfileName, false);

            if (profile == null)
                Console.WriteLine($"Failed to create create profile '{ProfileName}'");
            else
                Console.WriteLine($"Successfully created '{ProfileName}'");
        }
    }
}