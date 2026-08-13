namespace LAP.Application.DTO.Common
{
    using System;

    /// <summary>
    /// Represents a single entry in the leaderboard.
    /// </summary>
    public class LeaderboardEntryItemDto
    {
        /// <summary>
        /// Gets or sets the rank of the user on the leaderboard.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the weighted score of the user.
        /// </summary>
        public decimal WeightedScore { get; set; }

        /// <summary>
        /// Gets or sets the tier of the user on the leaderboard.
        /// </summary>
        public RefTermDto Tier { get; set; }
    }
}
