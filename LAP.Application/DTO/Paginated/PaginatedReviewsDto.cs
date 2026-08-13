using LAP.Application.DTO.Review;

namespace LAP.Application.DTO.Paginated
{
    /// <summary>
    /// Represents a paginated response containing a collection of course reviews.
    /// </summary>
    public class PaginatedReviewsDto
    {
        /// <summary>
        /// Gets or sets the collection of reviews for the current page.
        /// </summary>
        public ICollection<ReviewDto> Data { get; set; } = new List<ReviewDto>();

        /// <summary>
        /// Gets or sets the total number of reviews across all pages.
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
