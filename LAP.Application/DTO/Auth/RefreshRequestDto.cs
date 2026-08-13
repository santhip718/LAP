namespace LAP.Application.DTO.Auth
{
    /// <summary>
    /// Represents a request to refresh an expired access token using a valid refresh token.
    /// </summary>
    public class RefreshRequestDto
    {
        /// <summary>
        /// Gets or sets the refresh token used to generate a new access token.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
