namespace LAP.Shared.Exceptions
{
    using System;

    /// <summary>
    /// Serves as the base class for all custom exceptions in the application,
    /// providing additional fields such as a description and an HTTP status code.
    /// </summary>
    public class BaseCustomException : Exception
    {
        /// <summary>
        /// Gets or sets a detailed explanation of the error.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code associated with the exception.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCustomException"/> class.
        /// </summary>
        /// <param name="message">A short description of the error.</param>
        /// <param name="description">A detailed explanation of the error.</param>
        /// <param name="statusCode">The corresponding HTTP status code.</param>
        public BaseCustomException(string message, string description, int statusCode)
            : base(message)
        {
            Description = description;
            StatusCode = statusCode;
        }
    }
}
