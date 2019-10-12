using System.Globalization;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Benchmarks.Benchmarks
{
    [InProcess]
    public class EnumToStringBenchmark
    {
        [Benchmark]
        public string EnumsDotNet()
        {
            return AwsRegion.ApEast1.AsString(EnumFormat.EnumMemberValue);
        }

        [Benchmark]
        public string DotNet()
        {
            return AwsRegion.ApEast1.ToString(CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public string DotNetUpper()
        {
            return AwsRegion.ApEast1.ToString().ToUpperInvariant();
        }
    }
}