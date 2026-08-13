namespace LAP.Application.DTO.CourseReview
{
    /// <summary>
    /// Represents a request to update an existing review.
    /// </summary>
    public class UpdateReviewRequestDto
    {
        /// <summary>
        /// Gets or sets the rating value (e.g., 1 to 5).
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Gets or sets the text content of the review.
        /// </summary>
        public string? ReviewText { get; set; }
    }
}
