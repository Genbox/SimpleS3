using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[InProcess]
public class CharHelperBenchmarks
{
    [Benchmark]
    public bool CharRangeWithOr()
    {
        char c = 'f';
        return c >= 'a' || c <= '<';
    }

    [Benchmark]
    public bool CharRangeWithHelper()
    {
        char c = 'f';
        return CharHelper.InRange(c, 'a', 'z');
    }
}