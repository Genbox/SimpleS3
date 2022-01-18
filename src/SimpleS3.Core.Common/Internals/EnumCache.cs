using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genbox.SimpleS3.Core.Common.Internals;

internal class EnumCache<T> where T : Enum
{
    private readonly Dictionary<T, string> _map;
    private readonly Dictionary<string, T> _map2;
    private readonly Type _type;

    public EnumCache()
    {
        _type = typeof(T);
        string[] names = Enum.GetNames(_type);

        _map = new Dictionary<T, string>(names.Length);
        _map2 = new Dictionary<string, T>(names.Length, StringComparer.OrdinalIgnoreCase);

        foreach (string name in names)
        {
            T enumVal = (T)Enum.Parse(_type, name);
            string enumStr = GetAttributeValue(name) ?? name;

            _map.Add(enumVal, enumStr);
            _map2.Add(enumStr, enumVal);
        }
    }

    public static EnumCache<T> Instance { get; } = new EnumCache<T>();

    public string AsString(ref T value)
    {
        if (!_map.TryGetValue(value, out string str))
            throw new InvalidOperationException("Invalid enum value " + value);

        return str;
    }

    private string? GetAttributeValue(string name)
    {
        EnumValueAttribute attributes = _type.GetField(name).GetCustomAttribute<EnumValueAttribute>();
        return attributes?.Value;
    }

    public bool TryGetValueFromString(string value, out T enumVal)
    {
        return _map2.TryGetValue(value, out enumVal);
    }
}