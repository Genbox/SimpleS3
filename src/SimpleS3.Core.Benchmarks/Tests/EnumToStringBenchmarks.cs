using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.SimpleS3.Core.Benchmarks.Misc;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[InProcess]
public class EnumToStringBenchmarks
{
    [Benchmark]
    public string? EnumsDotNet()
    {
        return TestEnum.Value3.AsString(EnumFormat.EnumMemberValue);
    }

    [Benchmark]
    public string DotNet()
    {
        return TestEnum.Value3.ToString();
    }

    [Benchmark]
    public string Custom()
    {
        return EnumHelper.AsString(TestEnum.Value3);
    }
}