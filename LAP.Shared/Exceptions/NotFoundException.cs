namespace LAP.Shared.Exceptions
{
    using System.Net;

    /// <summary>
    /// Represents an exception that is thrown when a requested resource
    /// cannot be found in the system.
    /// </summary>
    public class NotFoundException : BaseCustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="message">A short description of the missing resource.</param>
        /// <param name="description">A detailed explanation of why the resource was not found.</param>
        public NotFoundException(string message, string description)
            : base(message, description, (int)HttpStatusCode.NotFound) { }
    }
}
