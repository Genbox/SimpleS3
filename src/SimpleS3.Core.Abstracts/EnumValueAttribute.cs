using System;

namespace Genbox.SimpleS3.Core.Abstracts
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumValueAttribute : Attribute
    {
        public EnumValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}