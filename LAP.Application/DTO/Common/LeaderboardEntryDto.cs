namespace LAP.Application.DTO.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a paginated leaderboard entry response.
    /// </summary>
    public class LeaderboardEntryDto
    {
        /// <summary>
        /// Gets or sets the collection of leaderboard entry items.
        /// </summary>
        public ICollection<LeaderboardEntryItemDto> Data { get; set; }

        /// <summary>
        /// Gets or sets the total number of entries available.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; }
    }
}
