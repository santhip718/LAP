using LAP.Application.Authorization;

namespace LAP.UnitTest.Helpers;

public class FeatureRequirementTest
{
    [Fact]
    public void Constructor_ShouldSetFeatureName()
    {
        var requirement = new FeatureRequirement("ViewCourses");

        Assert.Equal("ViewCourses", requirement.FeatureName);
    }

    [Fact]
    public void Constructor_ShouldHandleEmptyFeatureName()
    {
        var requirement = new FeatureRequirement("");

        Assert.Equal("", requirement.FeatureName);
    }

    [Fact]
    public void Constructor_ShouldHandleNullFeatureName()
    {
        var requirement = new FeatureRequirement(null!);

        Assert.Null(requirement.FeatureName);
    }

    [Fact]
    public void FeatureName_ShouldBeReadOnly()
    {
        var requirement = new FeatureRequirement("ViewCourses");

        var property = requirement.GetType().GetProperty("FeatureName");

        Assert.NotNull(property);
        Assert.False(property.CanWrite);
    }
}
