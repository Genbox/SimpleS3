using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands
{
    public abstract class ObjectOperationBase : CommandBase<S3Cli>
    {
        [Argument(0)]
        [Required]
        public string Source { get; set; }

        [Argument(1)]
        [Required]
        public string Destination { get; set; }
    }
}
