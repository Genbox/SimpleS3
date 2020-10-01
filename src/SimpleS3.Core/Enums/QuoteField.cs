﻿using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum QuoteField
    {
        Unknown = 0,

        [EnumMember(Value = "ALWAYS")]
        Always,

        [EnumMember(Value = "ASNEEDED")]
        AsNeeded
    }
}