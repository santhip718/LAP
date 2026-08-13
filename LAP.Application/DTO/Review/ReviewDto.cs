namespace LAP.Application.DTO.Review
{
    using System;

    /// <summary>
    /// Represents a review with full details.
    /// </summary>
    public class ReviewDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the review.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who wrote the review.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user who wrote the review.
        /// </summary>
        public string? UserFullName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the course being reviewed.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the rating value (e.g., 1 to 5).
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the text content of the review.
        /// </summary>
        public string? ReviewText { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the review was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }
    }
}
