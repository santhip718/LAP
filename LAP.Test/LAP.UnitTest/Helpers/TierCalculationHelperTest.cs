using LAP.Application.Helper;
using LAP.Domain.Entity;
using LAP.Shared.Exceptions;

namespace LAP.UnitTest.Helpers;

public class TierCalculationHelperTest
{
    [Theory]
    [InlineData(0, "Code Cadet")]
    [InlineData(25, "Syntax Voyager")]
    [InlineData(39.99, "Syntax Voyager")]
    [InlineData(40, "Syntax Voyager")]
    [InlineData(50, "Logic Architect")]
    [InlineData(59.99, "Logic Architect")]
    [InlineData(60, "Logic Architect")]
    [InlineData(70, "Runtime Titan")]
    [InlineData(79.99, "Runtime Titan")]
    [InlineData(80, "Runtime Titan")]
    [InlineData(90, "System Sovereign")]
    [InlineData(94.99, "System Sovereign")]
    [InlineData(95, "System Sovereign")]
    [InlineData(100, "System Sovereign")]
    public void GetTierName_ShouldReturnCorrectTier(decimal percentage, string expectedTier)
    {
        string result = TierCalculationHelper.GetTierName(percentage);

        Assert.Equal(expectedTier, result);
    }

    [Fact]
    public void GetTierId_ShouldReturnMatchingRefTermId()
    {
        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Code Cadet" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Syntax Voyager" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Logic Architect" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Runtime Titan" },
            new RefTerm { Id = Guid.NewGuid(), Name = "System Sovereign" },
        };

        Guid result = TierCalculationHelper.GetTierId(85, tiers);

        Assert.Equal(tiers[4].Id, result);
    }

    [Fact]
    public void GetTierId_ShouldThrowNotFoundException_WhenTierNotInCollection()
    {
        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Syntax Voyager" },
        };

        Assert.Throws<NotFoundException>(() => TierCalculationHelper.GetTierId(85, tiers));
    }

    [Fact]
    public void GetTierId_ShouldThrowArgumentNullException_WhenTiersIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => TierCalculationHelper.GetTierId(50, null!));
    }

    [Theory]
    [InlineData(8, 10, 80)]
    [InlineData(0, 10, 0)]
    [InlineData(10, 10, 100)]
    [InlineData(3, 7, 42.86)]
    [InlineData(1, 3, 33.33)]
    [InlineData(50, 0, 0)]
    [InlineData(0, 0, 0)]
    public void CalculatePercentage_ShouldReturnCorrectPercentage(
        decimal score,
        decimal totalMark,
        decimal expected
    )
    {
        decimal result = TierCalculationHelper.CalculatePercentage(score, totalMark);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateOverallTierId_ShouldReturnCorrectTier_WhenHistoriesExist()
    {
        Guid codeCadetId = Guid.NewGuid();
        Guid syntaxVoyagerId = Guid.NewGuid();
        Guid logicArchitectId = Guid.NewGuid();
        Guid runtimeTitanId = Guid.NewGuid();
        Guid systemSovereignId = Guid.NewGuid();

        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = codeCadetId, Name = "Code Cadet" },
            new RefTerm { Id = syntaxVoyagerId, Name = "Syntax Voyager" },
            new RefTerm { Id = logicArchitectId, Name = "Logic Architect" },
            new RefTerm { Id = runtimeTitanId, Name = "Runtime Titan" },
            new RefTerm { Id = systemSovereignId, Name = "System Sovereign" },
        };

        List<AssessmentHistory> histories = new()
        {
            new AssessmentHistory { CompletedOn = DateTime.UtcNow, WeightedScore = 60 },
            new AssessmentHistory { CompletedOn = DateTime.UtcNow, WeightedScore = 70 },
        };

        Guid result = TierCalculationHelper.CalculateOverallTierId(histories, tiers);

        Assert.Equal(runtimeTitanId, result);
    }

    [Fact]
    public void CalculateOverallTierId_ShouldUseCodeCadet_WhenNoCompletedHistories()
    {
        Guid codeCadetId = Guid.NewGuid();
        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = codeCadetId, Name = "Code Cadet" },
        };

        List<AssessmentHistory> histories = new()
        {
            new AssessmentHistory { CompletedOn = null, WeightedScore = 90 },
        };

        Guid result = TierCalculationHelper.CalculateOverallTierId(histories, tiers);

        Assert.Equal(codeCadetId, result);
    }

    [Fact]
    public void CalculateOverallTierId_ShouldThrowArgumentNullException_WhenHistoriesIsNull()
    {
        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Code Cadet" },
        };

        Assert.Throws<ArgumentNullException>(() =>
            TierCalculationHelper.CalculateOverallTierId(null!, tiers)
        );
    }

    [Fact]
    public void CalculateOverallTierId_ShouldThrowArgumentNullException_WhenTiersIsNull()
    {
        List<AssessmentHistory> histories = new()
        {
            new AssessmentHistory { CompletedOn = DateTime.UtcNow },
        };

        Assert.Throws<ArgumentNullException>(() =>
            TierCalculationHelper.CalculateOverallTierId(histories, null!)
        );
    }

    [Fact]
    public void CalculateOverallTierId_ShouldReturnCorrectTier_ForPerfectAverage()
    {
        Guid systemSovereignId = Guid.NewGuid();
        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Code Cadet" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Syntax Voyager" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Logic Architect" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Runtime Titan" },
            new RefTerm { Id = systemSovereignId, Name = "System Sovereign" },
        };

        List<AssessmentHistory> histories = new()
        {
            new AssessmentHistory { CompletedOn = DateTime.UtcNow, WeightedScore = 98 },
            new AssessmentHistory { CompletedOn = DateTime.UtcNow, WeightedScore = 96 },
        };

        Guid result = TierCalculationHelper.CalculateOverallTierId(histories, tiers);

        Assert.Equal(systemSovereignId, result);
    }

    [Fact]
    public void CalculateOverallTierId_ShouldReturnCorrectTier_ForLowAverage()
    {
        Guid syntaxVoyagerId = Guid.NewGuid();
        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Code Cadet" },
            new RefTerm { Id = syntaxVoyagerId, Name = "Syntax Voyager" },
        };

        List<AssessmentHistory> histories = new()
        {
            new AssessmentHistory { CompletedOn = DateTime.UtcNow, WeightedScore = 20 },
            new AssessmentHistory { CompletedOn = DateTime.UtcNow, WeightedScore = 30 },
        };

        Guid result = TierCalculationHelper.CalculateOverallTierId(histories, tiers);

        Assert.Equal(syntaxVoyagerId, result);
    }

    [Fact]
    public void GetTierId_ShouldRespectHighBoundary()
    {
        List<RefTerm> tiers = new()
        {
            new RefTerm { Id = Guid.NewGuid(), Name = "Code Cadet" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Syntax Voyager" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Logic Architect" },
            new RefTerm { Id = Guid.NewGuid(), Name = "Runtime Titan" },
        };

        Assert.Throws<NotFoundException>(() => TierCalculationHelper.GetTierId(95, tiers));
    }
}
