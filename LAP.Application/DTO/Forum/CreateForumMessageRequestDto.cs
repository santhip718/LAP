namespace LAP.Application.DTO.Forum
{
    /// <summary>
    /// Represents a request to create a new forum message.
    /// </summary>
    public class CreateForumMessageRequestDto
    {
        /// <summary>
        /// Gets or sets the text content of the forum message.
        /// </summary>
        public string? MessageText { get; set; }
    }
}
