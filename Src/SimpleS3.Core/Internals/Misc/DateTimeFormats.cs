namespace Genbox.SimpleS3.Core.Internals.Misc;

internal static class DateTimeFormats
{
    public const string Iso8601Date = "yyyyMMdd";
    public const string Iso8601DateTime = @"yyyyMMdd\THHmmssZ";
    public const string Iso8601DateTimeExtended = @"yyyy-MM-dd\THH:mm:ss.FFFFFFFK"; //F = up to N fractional seconds; K = Z or UTC offset
    public const string Rfc1123 = "R";
}