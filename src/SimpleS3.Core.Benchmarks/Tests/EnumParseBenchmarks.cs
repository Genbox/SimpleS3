using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Benchmarks.Misc;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[InProcess]
public class EnumParseBenchmarks
{
    [Benchmark]
    public bool EnumsDotNet()
    {
        return EnumsNET.Enums.TryParse<TestEnum>("Value3", true, out _);
    }

    [Benchmark]
    public bool DotNet()
    {
        return Enum.TryParse<TestEnum>("Value3", true, out _);
    }

    [Benchmark]
    public bool Custom()
    {
        return EnumHelper.TryParse<TestEnum>("Value3", out _);
    }
}