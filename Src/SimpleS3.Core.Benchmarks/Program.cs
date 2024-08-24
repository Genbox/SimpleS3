using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Genbox.SimpleS3.Core.Benchmarks;

internal static class Program
{
    private static void Main(string[] args)
    {
        IConfig config = DefaultConfig.Instance
                                      .AddJob(new Job(RunMode.Short, Job.InProcess))
                                      .WithOption(ConfigOptions.DisableLogFile, true);

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
    }
}