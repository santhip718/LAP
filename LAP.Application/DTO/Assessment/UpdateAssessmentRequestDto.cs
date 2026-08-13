namespace LAP.Application.DTO.Assessment
{
    /// <summary>
    /// Represents a request to update an existing assessment.
    /// </summary>
    public class UpdateAssessmentRequestDto
    {
        /// <summary>
        /// Gets or sets the updated assessment title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the updated assessment description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the updated total mark.
        /// </summary>
        public int? TotalMark { get; set; }

        /// <summary>
        /// Gets or sets the updated passing mark.
        /// </summary>
        public int? PassingMark { get; set; }

        /// <summary>
        /// Gets or sets the updated duration in minutes.
        /// </summary>
        public int? DurationMinute { get; set; }
    }
}
