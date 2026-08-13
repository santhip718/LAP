using LAP.Domain.Entity;
using LAP.Shared.Exceptions;
using LAP.Application.Constant;

namespace LAP.Application.Helper;

/// <summary>
/// Provides tier calculation logic for the Learning &amp; Assessment Portal.
/// Maps percentage scores to tier names/IDs and computes overall user tiers.
/// </summary>
public static class TierCalculationHelper
{
    /// <summary>
    /// Returns the tier name corresponding to the given percentage score.
    /// </summary>
    /// <param name="percentage">The percentage score.</param>
    /// <returns>The tier name.</returns>
    public static string GetTierName(decimal percentage)
    {
        return percentage switch
        {
            <= 20 => CommonConstants.TIER_CODE_CADET,
            <= 40 => CommonConstants.TIER_SYNTAX_VOYAGER,
            <= 60 => CommonConstants.TIER_LOGIC_ARCHITECT,
            <= 80 => CommonConstants.TIER_RUNTIME_TITAN,
            _ => CommonConstants.TIER_SYSTEM_SOVEREIGN,
        };
    }

    /// <summary>
    /// Returns the <see cref="RefTerm"/> identifier for the tier matching the given percentage.
    /// </summary>
    /// <param name="percentage">The percentage score.</param>
    /// <param name="tiers">The collection of tier reference terms.</param>
    /// <returns>The matching tier's identifier.</returns>
    /// <exception cref="NotFoundException">Thrown when no tier matches the given percentage.</exception>
    public static Guid GetTierId(decimal percentage, IEnumerable<RefTerm> tiers)
    {
        ArgumentNullException.ThrowIfNull(tiers);

        string tierName = GetTierName(percentage);

        RefTerm? tier =
            tiers.FirstOrDefault(t => t.Name == tierName)
            ?? throw new NotFoundException(
                "Tier not found",
                $"Tier '{tierName}' not found in the provided reference terms."
            );

        return tier.Id;
    }

    /// <summary>
    /// Calculates the percentage from a raw score and total mark.
    /// </summary>
    /// <param name="score">The obtained score.</param>
    /// <param name="totalMark">The total possible mark.</param>
    /// <returns>The percentage rounded to two decimal places.</returns>
    public static decimal CalculatePercentage(decimal score, decimal totalMark)
    {
        if (totalMark == 0)
        {
            return 0;
        }

        return Math.Round(score / totalMark * 100, 2);
    }

    /// <summary>
    /// Calculates the overall tier identifier based on the average weighted score across all completed assessments.
    /// </summary>
    /// <param name="assessmentHistories">The collection of assessment attempt histories.</param>
    /// <param name="tiers">The collection of tier reference terms.</param>
    /// <returns>The overall tier identifier.</returns>
    public static Guid CalculateOverallTierId(
        IEnumerable<AssessmentHistory> assessmentHistories,
        IEnumerable<RefTerm> tiers
    )
    {
        ArgumentNullException.ThrowIfNull(assessmentHistories);
        ArgumentNullException.ThrowIfNull(tiers);

        List<AssessmentHistory> completedHistories = assessmentHistories
            .Where(h => h.CompletedOn.HasValue)
            .ToList();

        if (completedHistories.Count == 0)
        {
            return GetTierId(0, tiers);
        }

        decimal averageScore = completedHistories.Average(h => h.WeightedScore);

        return GetTierId(averageScore, tiers);
    }
}
