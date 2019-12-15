using System.Text;
using BenchmarkDotNet.Attributes;

namespace Genbox.SimpleS3.Core.Benchmarks
{
    [InProcess]
    public class StringBuilderBenchmarks
    {
        private readonly StringBuilder _append = new StringBuilder();
        private readonly StringBuilder _appendFormat = new StringBuilder();

        [IterationCleanup]
        public void IterationCleanup()
        {
            _append.Clear();
            _appendFormat.Clear();
        }

        [Benchmark]
        public StringBuilder Append()
        {
            return _append.Append("MyKey").Append('=').Append("MyValue");
        }

        [Benchmark]
        public StringBuilder AppendFormat()
        {
            return _appendFormat.AppendFormat("{0}={1}", "MyKey", "MyValue");
        }
    }
}