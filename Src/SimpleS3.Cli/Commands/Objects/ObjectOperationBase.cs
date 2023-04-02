using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

public abstract class ObjectOperationBase : OnlineCommandBase
{
    [Argument(0)]
    [Required]
    public string Source { get; set; } = null!;

    [Argument(1)]
    [Required]
    public string Destination { get; set; } = null!;
}