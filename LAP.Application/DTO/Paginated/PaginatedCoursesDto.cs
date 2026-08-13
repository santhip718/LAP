namespace LAP.Application.DTO.Paginated
{
    using System.Collections.Generic;
    using LAP.Application.DTO.Course;

    /// <summary>
    /// Represents a paginated response containing a collection of courses.
    /// </summary>
    public class PaginatedCoursesDto
    {
        /// <summary>
        /// Gets or sets the collection of course summaries for the current page.
        /// </summary>
        public ICollection<CourseSummaryDto> Data { get; set; }

        /// <summary>
        /// Gets or sets the total number of courses across all pages.
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
