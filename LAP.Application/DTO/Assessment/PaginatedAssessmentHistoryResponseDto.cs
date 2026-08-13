namespace LAP.Application.DTO.Assessment
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a paginated list of assessment history records for a user.
    /// </summary>
    public class PaginatedAssessmentHistoryResponseDto
    {
        /// <summary>
        /// Gets or sets the collection of assessment history records for the current page.
        /// </summary>
        public ICollection<AssessmentHistoryItemDto> Item { get; set; } =
            new List<AssessmentHistoryItemDto>();

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of records per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of records across all pages.
        /// </summary>
        public int TotalRecords { get; set; }
    }
}
