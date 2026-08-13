namespace LAP.Application.DTO.Assessment
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a paginated list of assessment history records.
    /// </summary>
    public class PaginatedAssessmentHistoryDto
    {
        /// <summary>
        /// Gets or sets the collection of assessment history records for the current page.
        /// </summary>
        public ICollection<AssessmentHistoryDto>? Data { get; set; }

        /// <summary>
        /// Gets or sets the total number of records across all pages.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the number of records per page.
        /// </summary>
        public int PageSize { get; set; }
    }
}
