using System.Globalization;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[MemoryDiagnoser]
public class KeyValueTupleBenchmarks
{
    private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();

    public KeyValueTupleBenchmarks()
    {
        Random r = new Random(42);

        for (int i = 0; i < 100; i++)
            _dict.Add(r.Next(10000, int.MaxValue).ToString(NumberFormatInfo.InvariantInfo), r.Next(10000, int.MaxValue).ToString(NumberFormatInfo.InvariantInfo));
    }

    [Benchmark]
    public IList<(string, string)> ToTuples()
    {
        return _dict.Select(x => (x.Key, x.Value)).ToList();
    }

    [Benchmark]
    public IList<KeyValuePair<string, string>> ToKeyValue()
    {
        List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

        foreach (KeyValuePair<string, string> pair in _dict)
            list.Add(pair);

        return list;
    }
}