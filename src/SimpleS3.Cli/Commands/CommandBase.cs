using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands
{
    [HelpOption("--help")]
    public abstract class CommandBase<T>
    {
        protected T Parent { get; set; }

        protected CliManager Manager { get; private set; }

        protected abstract Task ExecuteAsync(CommandLineApplication app, CancellationToken token);

        internal async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            S3Cli mainParent = null;

            if (app is CommandLineApplication<S3Cli> cliApp)
                mainParent = cliApp.Model;
            else if (app.Parent is CommandLineApplication<S3Cli> cliApp2)
                mainParent = cliApp2.Model;
            else if (app.Parent.Parent is CommandLineApplication<S3Cli> cliApp3)
                mainParent = cliApp3.Model;

            Manager = CliManager.GetCliManager(mainParent.ProfileName, mainParent.Region);

            try
            {
                await ExecuteAsync(app, token).ConfigureAwait(false);
                return 0;
            }
            catch (Exception e)
            {
                return 1;
            }
        }
    }
}