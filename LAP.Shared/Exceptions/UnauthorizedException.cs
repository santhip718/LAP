namespace LAP.Shared.Exceptions
{
    using System.Net;

    /// <summary>
    /// Represents an exception thrown when a user attempts to access a resource
    /// without proper authentication.
    /// </summary>
    public class UnauthorizedException : BaseCustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">A short message describing the authentication failure.</param>
        /// <param name="description">A detailed description explaining why authentication failed.</param>
        public UnauthorizedException(string message, string description)
            : base(message, description, (int)HttpStatusCode.Unauthorized) { }
    }
}
