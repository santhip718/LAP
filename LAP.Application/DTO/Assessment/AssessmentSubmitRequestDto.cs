namespace LAP.Application.DTO.Assessment
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using LAP.Application.DTO.Common;

    /// <summary>
    /// Represents a request to submit an assessment with provided answers.
    /// </summary>
    public class AssessmentSubmitRequestDto
    {
        /// <summary>
        /// Gets or sets the user identifier submitting the assessment.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the assessment was started.
        /// </summary>
        public DateTime StartedOn { get; set; }

        /// <summary>
        /// Gets or sets the collection of answers submitted by the user.
        /// </summary>
        public ICollection<Answer> Answer { get; set; } = new Collection<Answer>();
    }
}
