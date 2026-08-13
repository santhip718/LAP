namespace LAP.Application.DTO.Assessment
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the assessment result with all attempt history.
    /// </summary>
    public class AssessmentResultResponseDto
    {
        /// <summary>
        /// Gets or sets the assessment identifier.
        /// </summary>
        public Guid AssessmentId { get; set; }

        /// <summary>
        /// Gets or sets the assessment title.
        /// </summary>
        public string AssessmentTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the passing mark required to pass the assessment.
        /// </summary>
        public int PassingMark { get; set; }

        /// <summary>
        /// Gets or sets the list of all attempt results for this assessment.
        /// </summary>
        public List<AssessmentAttemptDto> Attempts { get; set; } = new();
    }
}
