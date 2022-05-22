// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Cli.CommandLineUtils;

internal sealed class CommandLineService<T> : IDisposable, ICommandLineService where T : class
{
    private readonly CommandLineApplication _application;
    private readonly ILogger _logger;
    private readonly CommandLineState _state;

    /// <summary>Creates a new instance.</summary>
    /// <param name="logger">A logger</param>
    /// <param name="state">The command line state</param>
    /// <param name="serviceProvider">The DI service provider</param>
    public CommandLineService(ILogger<CommandLineService<T>> logger, CommandLineState state, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _state = state;

        logger.LogDebug("Constructing CommandLineApplication<{type}> with args [{args}]", typeof(T).FullName, string.Join(",", state.Arguments));
        _application = new CommandLineApplication<T>(state.Console, state.WorkingDirectory);
        _application.Conventions
                    .UseAttributes()

                    //.SetAppNameFromEntryAssembly()
                    //.SetRemainingArgsPropertyOnModel()
                    .SetSubcommandPropertyOnModel()
                    .SetParentPropertyOnModel()
                    .UseOnExecuteMethodFromModel()

                    //.UseOnValidateMethodFromModel()
                    //.UseOnValidationErrorMethodFromModel()
                    .UseDefaultHelpOption()
                    .UseCommandNameFromModelType()
                    .UseConstructorInjection(serviceProvider);

        foreach (IConvention convention in serviceProvider.GetServices<IConvention>())
            _application.Conventions.AddConvention(convention);
    }

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Running");
        _state.ExitCode = await _application.ExecuteAsync(_state.Arguments, cancellationToken).ConfigureAwait(false);
        return _state.ExitCode;
    }

    public void Dispose()
    {
        _application.Dispose();
    }
}