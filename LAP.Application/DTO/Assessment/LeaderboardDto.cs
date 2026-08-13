namespace LAP.Application.DTO.Assessment;

/// <summary>
/// DTO representing a leaderboard entry for a user.
/// </summary>
public class LeaderboardDto
{
    /// <summary>Gets or sets the unique identifier for the user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the full name of the user.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Gets or sets the weighted score used for ranking.</summary>
    public decimal OverallWeightedScore { get; set; }

    /// <summary>Gets or sets the weighted score for the user.</summary>
    public decimal WeightedScore { get; set; }

    /// <summary>Gets or sets the rank of the user on the leaderboard.</summary>
    public int Rank { get; set; }
}
