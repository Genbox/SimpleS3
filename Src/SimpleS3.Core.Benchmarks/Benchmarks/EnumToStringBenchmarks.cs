using EnumsNET;
using Genbox.SimpleS3.Core.Benchmarks.Misc;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Benchmarks.Benchmarks;

public class EnumToStringBenchmarks
{
    [Benchmark]
    public string? EnumsDotNet() => TestEnum.Value3.AsString(EnumFormat.EnumMemberValue);

    [Benchmark]
    public string DotNet() => TestEnum.Value3.ToString();

    [Benchmark]
    public string Custom() => EnumHelper.AsString(TestEnum.Value3);
}