namespace LAP.Shared.Exceptions
{
    using System.Net;

    /// <summary>
    /// Represents an exception thrown when a user attempts to access a resource
    /// for which they do not have sufficient permissions.
    /// </summary>
    public class ForbiddenException : BaseCustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">A short description of the access violation.</param>
        /// <param name="description">A detailed explanation of why access is denied.</param>
        public ForbiddenException(string message, string description)
            : base(message, description, (int)HttpStatusCode.Forbidden) { }
    }
}
