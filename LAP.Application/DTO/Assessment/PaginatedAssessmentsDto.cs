namespace LAP.Application.DTO.Assessment
{
    using System.Collections.Generic;
    using LAP.Application.DTO.Course;

    /// <summary>
    /// Represents a paginated list of assessment overviews.
    /// </summary>
    public class PaginatedAssessmentsDto
    {
        /// <summary>
        /// Gets or sets the collection of assessment overviews for the current page.
        /// </summary>
        public ICollection<AssessmentOverviewDto>? Data { get; set; }

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
