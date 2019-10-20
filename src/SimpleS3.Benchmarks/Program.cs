using BenchmarkDotNet.Running;

namespace Genbox.SimpleS3.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}