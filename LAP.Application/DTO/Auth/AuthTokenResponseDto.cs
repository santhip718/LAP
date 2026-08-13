namespace LAP.Application.DTO.Auth
{
    /// <summary>
    /// Represents the response returned after successful authentication, containing access and refresh tokens.
    /// </summary>
    public class AuthTokenResponseDto
    {
        /// <summary>
        /// Gets or sets the JWT access token used for authenticating API requests.
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token used to obtain a new access token when the current one expires.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the lifetime of the access token in seconds.
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
