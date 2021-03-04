using System.Text;
using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Common.Pools;

namespace Genbox.SimpleS3.Core.Benchmarks
{
    [InProcess]
    public class StringBuilderBenchmarks
    {
        private readonly StringBuilder _append = new StringBuilder();
        private readonly StringBuilder _appendFormat = new StringBuilder();
        public string EqualSign = "=";

        [Benchmark]
        public string Append()
        {
            _append.Clear();
            return _append.Append("MyKey").Append('=').Append("MyValue").ToString();
        }

        [Benchmark]
        public string AppendFormat()
        {
            _appendFormat.Clear();
            return _appendFormat.AppendFormat("{0}={1}", "MyKey", "MyValue").ToString();
        }

        [Benchmark]
        public string NormalAdd()
        {
            //We use a field here to avoid a smart compiler from creating a constant
            return "MyKey" + EqualSign + "MyValue";
        }

        [Benchmark]
        public string Interpolation()
        {
            //We use a field here to avoid a smart compiler from creating a constant
            return $"MyKey{EqualSign}MyValue";
        }

        [Benchmark]
        public string FromPool()
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();

            //We use a field here to avoid a smart compiler from creating a constant
            sb.Append("MyKey").Append(EqualSign).Append("MyValue");

            return StringBuilderPool.Shared.ReturnString(sb);
        }
    }
}