using Genbox.SimpleS3.Core.Benchmarks.Misc;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[InProcess]
public class EnumParseBenchmarks
{
    [Benchmark]
    public bool EnumsDotNet() => EnumsNET.Enums.TryParse<TestEnum>("Value3", true, out _);

    [Benchmark]
    public bool DotNet() => Enum.TryParse<TestEnum>("Value3", true, out _);

    [Benchmark]
    public bool Custom() => EnumHelper.TryParse<TestEnum>("Value3", out _);
}