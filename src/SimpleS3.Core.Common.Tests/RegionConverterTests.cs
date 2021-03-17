using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Tests.Code;
using Xunit;

namespace Genbox.SimpleS3.Core.Common.Tests
{
    public class RegionConverterTests
    {
        private readonly IRegionConverter _converter;

        public RegionConverterTests()
        {
            _converter = new RegionConverter(new TestRegionData());
        }

        [Fact]
        public void EnumToString()
        {
            Assert.Equal("Region-One", _converter.GetRegion(TestRegion.RegionOne).Code);
            Assert.Equal("Region-Two", _converter.GetRegion(TestRegion.RegionTwo).Code);

            Assert.Equal("Region-Two", _converter.GetRegion((int)TestRegion.RegionTwo).Code);
        }

        [Fact]
        public void StringToEnum()
        {
            Assert.Equal(TestRegion.RegionOne, _converter.GetRegion("Region-One").EnumValue);
            Assert.Equal(TestRegion.RegionTwo, _converter.GetRegion("Region-Two").EnumValue);

            //Should be case-insensitive
            Assert.Equal(TestRegion.RegionTwo, _converter.GetRegion("region-two").EnumValue);
        }
    }
}
