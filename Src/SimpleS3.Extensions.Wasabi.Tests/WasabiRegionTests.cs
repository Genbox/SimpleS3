using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Extensions.Wasabi.Tests;

public class WasabiRegionTests
{
    [Theory]
    [InlineData(WasabiRegion.UsWest2, "us-west-2", 15)]
    [InlineData(WasabiRegion.EuWest3, "eu-west-3", 16)]
    public void NewRegionsMapToDocumentedCodes(WasabiRegion region, string code, int value)
    {
        ArgumentNullException.ThrowIfNull(code);
        RegionConverter converter = new RegionConverter(WasabiRegionData.Instance);
        Assert.Equal(value, (int)region);
        Assert.Equal(code, converter.GetRegion((int)region).Code);
        Assert.Equal(region, converter.GetRegion(code.ToUpperInvariant()).EnumValue);
    }

    [Fact]
    public void ExistingRegionNumericValuesRemainStable()
    {
        Assert.Equal(0, (int)WasabiRegion.Unknown);
        Assert.Equal(1, (int)WasabiRegion.UsEast1);
        Assert.Equal(14, (int)WasabiRegion.ApSouthEast2);
    }
}