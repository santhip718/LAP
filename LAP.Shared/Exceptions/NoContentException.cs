namespace LAP.Shared.Exceptions
{
    using System.Net;

    public class NoContentException : BaseCustomException
    {
        public NoContentException(string message, string description)
            : base(message, description, (int)HttpStatusCode.NoContent) { }
    }
}
