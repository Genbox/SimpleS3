using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class PropertyHelperTests
{
    [Fact]
    public void TestDefaultValues()
    {
        //User creates a config and sets OtherValue. We should not have an issue with the fact that it contains the value
        Config a = new Config();

        //We check that the state of configs is as taken from BaseConfig
        Assert.Null(a.NullDefaultValue);
        Assert.Equal(0, a.IntDefaultValue);
        Assert.Equal("new value", a.StringCtorValue); //This should be "new value" because it overwrites the base value in ctor
        Assert.Equal(42, a.IntCtorValue);

        //We create a base config and would like values to come from user config, but only if they are not the default value
        BaseConfig b = new BaseConfig();
        b.NullDefaultValue = "value from BaseConfig";

        PropertyHelper.MapObjects(a, b);

        //Check that the values were all correctly transferred (or rather, not transferred, because they are all the default values)
        //Here we would also crash if OtherValue would cause any issues
        Assert.Equal("value from BaseConfig", b.NullDefaultValue);
        Assert.Equal(0, b.IntDefaultValue);
        Assert.Equal("new value", b.StringCtorValue);
        Assert.Equal(42, b.IntCtorValue);

        //Now we change the user config and overwrite one of the inherited properties
        a.NullDefaultValue = "not null anymore";
        a.IntDefaultValue = 1;
        a.StringCtorValue = "different value";
        a.IntCtorValue = 100;

        //Transfer the values from A to a new B.
        b = new BaseConfig();
        PropertyHelper.MapObjects(a, b);

        Assert.Equal("not null anymore", b.NullDefaultValue);
        Assert.Equal(1, b.IntDefaultValue);
        Assert.Equal("different value", b.StringCtorValue);
        Assert.Equal(100, b.IntCtorValue);
    }

    private class BaseConfig
    {
        public string? NullDefaultValue { get; set; }
        public int IntDefaultValue { get; set; }
        public string StringCtorValue { get; set; } = "my value";
        public int IntCtorValue { get; set; } = 42;
    }

    private class Config : BaseConfig
    {
        public Config()
        {
            StringCtorValue = "new value";
        }
    }
}