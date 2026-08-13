namespace LAP.Shared.Exceptions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    /// <summary>
    /// Represents an exception that is thrown when an unexpected or unhandled
    /// server-side error occurs during request processing.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InternalServerException : BaseCustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerException"/> class.
        /// </summary>
        /// <param name="message">A short summary of the server error.</param>
        /// <param name="description">A detailed explanation of the error cause.</param>
        public InternalServerException(string message, string description)
            : base(message, description, (int)HttpStatusCode.InternalServerError) { }
    }
}
