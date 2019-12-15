using System.Globalization;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Benchmarks
{
    [InProcess]
    public class EnumToStringBenchmarks
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