// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Genbox.SimpleS3.Cli.CommandLineUtils;

/// <summary>A service to be run as part of the <see cref="CommandLineLifetime" />.</summary>
internal interface ICommandLineService
{
    /// <summary>Runs the application asynchronously and returns the exit code.</summary>
    /// <param name="cancellationToken">Used to indicate when stop should no longer be graceful.</param>
    /// <returns>The exit code</returns>
    Task<int> RunAsync(CancellationToken cancellationToken);
}