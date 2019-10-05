using BenchmarkDotNet.Running;
using Genbox.SimpleS3.Benchmarks.Benchmarks;

namespace Genbox.SimpleS3.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<EnumToStringBenchmark>();
        }
    }
}