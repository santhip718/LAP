namespace LAP.Shared.Exceptions
{
    using System.Net;

    /// <summary>
    /// Represents an exception that should be thrown when a bad request occurs,
    /// typically due to invalid user input or malformed data.
    /// </summary>
    public class BadRequestException : BaseCustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        /// <param name="message">A short description of the error.</param>
        /// <param name="description">A detailed explanation of the issue.</param>
        public BadRequestException(string message, string description)
            : base(message, description, (int)HttpStatusCode.BadRequest) { }
    }
}
