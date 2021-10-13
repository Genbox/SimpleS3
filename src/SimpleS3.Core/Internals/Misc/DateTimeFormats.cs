namespace Genbox.SimpleS3.Core.Internals.Misc
{
    internal static class DateTimeFormats
    {
        public const string Iso8601Date = "yyyyMMdd";
        public const string Iso8601DateTime = @"yyyyMMdd\THHmmssZ";
        public const string Iso8601DateTimeExtended = @"yyyy-MM-dd\THH:mm:ss.FFFFFFFZ"; //F = up to N fractional seconds
        public const string Rfc1123 = "R";
    }
}