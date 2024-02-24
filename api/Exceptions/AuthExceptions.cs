using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class UserNotFoundException : BaseCustomExceptions
    {
        public UserNotFoundException(string message) : base(message)
        {
        }
    }

    public class UserNotActiveException : BaseCustomExceptions
    {
        public UserNotActiveException(string message) : base(message)
        {
        }
    }

    public class UserLockedException : BaseCustomExceptions
    {
        public UserLockedException(string message) : base(message)
        {
        }
    }

    public class InvalidCredentialsException : BaseCustomExceptions
    {
        public InvalidCredentialsException(string message) : base(message)
        {
        }
    }
    public class UserAlreadyExistException : BaseCustomExceptions
    {
        public UserAlreadyExistException(string message) : base(message)
        {
        }
    }
    public class TokenInvalidException : BaseCustomExceptions
    {
        public TokenInvalidException(string message) : base(message)
        {
        }
    }
}