using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class PropertyHelperTests
{
    [Fact]
    public void TestDefaultValues()
    {
        // The derived configuration supplies values for unchanged destination properties.
        Config a = new Config();

        //We check that the state of configs is as taken from BaseConfig
        Assert.Null(a.NullDefaultValue);
        Assert.Equal(0, a.IntDefaultValue);
        Assert.Equal("new value", a.StringCtorValue); //This should be "new value" because it overwrites the base value in ctor
        Assert.Equal(42, a.IntCtorValue);

        // Explicit destination values must survive mapping.
        BaseConfig b = new BaseConfig();
        b.NullDefaultValue = "value from BaseConfig";
        b.IntDefaultValue = 7;

        PropertyHelper.MapObjects(a, b);

        // Preserve explicit destination values and populate its unchanged properties.
        Assert.Equal("value from BaseConfig", b.NullDefaultValue);
        Assert.Equal(7, b.IntDefaultValue);
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

    [Fact]
    public void MappingUsesSharedContractWhenDerivedPropertyHidesBaseProperty()
    {
        HiddenConfig source = new HiddenConfig { IntDefaultValue = 99 };
        ((BaseConfig)source).IntDefaultValue = 23;
        BaseConfig destination = new BaseConfig();

        PropertyHelper.MapObjects(source, destination);

        Assert.Equal(23, destination.IntDefaultValue);
        Assert.Equal(99, source.IntDefaultValue);
    }

    [Fact]
    public void MappingSkipsPropertiesWithoutReadableWritableValues()
    {
        AccessorConfig source = new AccessorConfig { Value = 17 };
        AccessorConfig destination = new AccessorConfig();

        PropertyHelper.MapObjects(source, destination);

        Assert.Equal(17, destination.Value);
        Assert.Equal(17, source.Value);
    }

    private sealed class HiddenConfig : BaseConfig
    {
        public new int IntDefaultValue { get; set; }
    }

    private sealed class AccessorConfig
    {
        public int Value { get; set; }
        public int ReadOnly => throw new InvalidOperationException($"Read-only property must not be evaluated for {Value}.");
        public int WriteOnly { set => throw new InvalidOperationException($"Write-only property must not be assigned for {Value}."); }

        public int this[int index]
        {
            get => throw new InvalidOperationException("Indexer must not be evaluated.");
            set => throw new InvalidOperationException("Indexer must not be assigned.");
        }
    }

    private class BaseConfig
    {
        public string? NullDefaultValue { get; set; }
        public int IntDefaultValue { get; set; }
        public string StringCtorValue { get; set; } = "my value";
        public int IntCtorValue { get; set; } = 42;
    }

    private sealed class Config : BaseConfig
    {
        public Config()
        {
            StringCtorValue = "new value";
        }
    }
}