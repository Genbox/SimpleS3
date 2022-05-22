// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace Genbox.SimpleS3.Cli.CommandLineUtils;

/// <summary>A DI container for storing command line arguments.</summary>
internal class CommandLineState : CommandLineContext
{
    public CommandLineState(string[] args)
    {
        Arguments = args;
    }

    public int ExitCode { get; set; }

    internal void SetConsole(IConsole console)
    {
        Console = console;
    }
}