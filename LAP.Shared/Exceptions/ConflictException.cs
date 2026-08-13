namespace LAP.Shared.Exceptions
{
    using System.Net;

    /// <summary>
    /// Represents an exception that is thrown when a conflict occurs,
    /// typically due to duplicate data or a resource already existing.
    /// </summary>
    public class ConflictException : BaseCustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException"/> class.
        /// </summary>
        /// <param name="message">A short description of the conflict.</param>
        /// <param name="description">A detailed explanation of the issue.</param>
        public ConflictException(string message, string description)
            : base(message, description, (int)HttpStatusCode.Conflict) { }
    }
}
