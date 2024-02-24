namespace api.Exceptions
{
    public class BaseCustomExceptions : Exception
    {
        public BaseCustomExceptions(string message) : base(message)
        {
        }
    }

    public class RequestInvalidException : BaseCustomExceptions
    {
        public RequestInvalidException(string message) : base(message)
        {
        }
    }
}