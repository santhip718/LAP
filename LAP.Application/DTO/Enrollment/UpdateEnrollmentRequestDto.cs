namespace LAP.Application.DTO.Enrollment
{
    /// <summary>
    /// Represents a request to update an enrollment's status.
    /// </summary>
    public class UpdateEnrollmentRequestDto
    {
        /// <summary>
        /// Gets or sets the updated enrollment status.
        /// </summary>
        public bool EnrollmentStatus { get; set; }
    }
}
