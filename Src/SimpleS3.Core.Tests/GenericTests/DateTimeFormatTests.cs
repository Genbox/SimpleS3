using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class DateTimeFormatTests
{
    [Theory]
    [InlineData("2026-09-13T12:34:56Z", 12, 0)]
    [InlineData("2026-09-13T12:34:56.1234567Z", 12, 1234567)]
    [InlineData("2026-09-13T12:34:56+02:00", 10, 0)]
    [InlineData("2026-09-13T12:34:56.1234567+02:00", 10, 1234567)]
    public void ExtendedIsoFormatParsesZuluAndOffsets(string value, int utcHour, int ticksWithinSecond)
    {
        DateTimeOffset parsed = ValueHelper.ParseDate(value, DateTimeFormat.Iso8601DateTimeExt);

        Assert.Equal(TimeSpan.Zero, parsed.Offset);
        Assert.Equal(utcHour, parsed.Hour);
        Assert.Equal(ticksWithinSecond, parsed.Ticks % TimeSpan.TicksPerSecond);
    }

    [Fact]
    public void ExtendedIsoFormatWritesDateTimeOffsetsAsZulu()
    {
        DateTimeOffset value = new DateTimeOffset(2026, 9, 13, 12, 34, 56, TimeSpan.FromHours(2)).AddTicks(1234567);
        HeadObjectRequest request = new HeadObjectRequest("bucket", "key");
        request.SetHeader("x-test-date", value, DateTimeFormat.Iso8601DateTimeExt);

        Assert.Equal("2026-09-13T10:34:56.1234567Z", ValueHelper.DateToString(value, DateTimeFormat.Iso8601DateTimeExt));
        Assert.Equal("2026-09-13T10:34:56.1234567Z", request.Headers["x-test-date"]);
    }
}