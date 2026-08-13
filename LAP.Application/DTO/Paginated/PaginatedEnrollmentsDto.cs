namespace LAP.Application.DTO.Paginated
{
    using System.Collections.Generic;
    using LAP.Application.DTO.Enrollment;

    /// <summary>
    /// Represents a paginated response containing a collection of enrollment details.
    /// </summary>
    public class PaginatedEnrollmentsDto
    {
        /// <summary>
        /// Gets or sets the collection of enrollment details for the current page.
        /// </summary>
        public ICollection<EnrollmentDetailDto> Data { get; set; }

        /// <summary>
        /// Gets or sets the total number of enrollments across all pages.
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
