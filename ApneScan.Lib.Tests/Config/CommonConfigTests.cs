using System.Reflection;
using System.Xml.Serialization;
using ApneScan.Sdk.Tests;
using ApneScan.Serialization;
using Xunit;

namespace ApneScan.Lib.Tests.Config;

public class CommonConfigTests : ContextualTests
{
    [Fact]
    public void CanSerialize()
    {
        var config = InternalDefaults.GetCommonConfig();
        config.FormStates = config.FormStates.Add(new FormState { Name = "Test" });
        config.BackgroundOperations = config.BackgroundOperations.Add("abc");

        var xml = config.ToXml();
        var config2 = xml.FromXml<CommonConfig>();

        Assert.Single(config2.FormStates);
        Assert.Equal("Test", config2.FormStates[0].Name);
        Assert.Single(config2.BackgroundOperations);
        Assert.Equal("abc", config2.BackgroundOperations.Single());
    }

    [Fact]
    public void AllHaveOrderMetadata()
    {
        foreach (var prop in typeof(CommonConfig).GetProperties())
        {
            var attribute = prop.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault();
            Assert.NotNull(attribute);
            Assert.InRange(attribute.Order, 1, int.MaxValue);
        }
    }
}