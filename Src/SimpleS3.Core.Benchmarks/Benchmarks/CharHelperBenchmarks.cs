using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Benchmarks.Benchmarks;

[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
public class CharHelperBenchmarks
{
    [Benchmark]
    public bool CharRangeWithOr()
    {
        char c = 'f';
        return c is >= 'a' or <= '<';
    }

    [Benchmark]
    public bool CharRangeWithHelper()
    {
        char c = 'f';
        return CharHelper.InRange(c, 'a', 'z');
    }
}