namespace LAP.Application.DTO.Forum
{
    using System;

    /// <summary>
    /// Represents a forum message with full details.
    /// </summary>
    public class ForumMessageDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the forum message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the course this forum message belongs to.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who posted the message.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user who posted the message.
        /// </summary>
        public string? UserFullName { get; set; }

        /// <summary>
        /// Gets or sets the text content of the forum message.
        /// </summary>
        public string? MessageText { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the message was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }
    }
}
