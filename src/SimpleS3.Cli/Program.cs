using System.Threading.Tasks;
using Genbox.SimpleS3.Cli.CommandLineUtils;
using Genbox.SimpleS3.Cli.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Cli
{
    internal class Program
    {
        private static Task<int> Main(string[] args)
        {
            return new HostBuilder()
                   .ConfigureLogging((context, builder) => builder.AddConsole())
                   .RunCommandLineApplicationAsync<S3Cli>(args);
        }
    }
}