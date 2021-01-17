﻿using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.SimpleS3.Extensions.AwsS3;

namespace Genbox.SimpleS3.Core.Benchmarks
{
    [InProcess]
    public class EnumToStringBenchmarks
    {
        [Benchmark]
        public string? EnumsDotNet()
        {
            return AwsRegion.ApEast1.AsString(EnumFormat.EnumMemberValue);
        }

        [Benchmark]
        public string DotNet()
        {
            return AwsRegion.ApEast1.ToString();
        }

        [Benchmark]
        public string DotNetUpper()
        {
            return AwsRegion.ApEast1.ToString().ToUpperInvariant();
        }
    }
}