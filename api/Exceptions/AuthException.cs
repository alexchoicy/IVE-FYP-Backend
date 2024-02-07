using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class AuthException
    {
        public class UserNotFoundException : Exception
        {
            public UserNotFoundException(string message) : base(message)
            {
            }
        }

        public class UserNotActiveException : Exception
        {
            public UserNotActiveException(string message) : base(message)
            {
            }
        }

        public class UserLockedException : Exception
        {
            public UserLockedException(string message) : base(message)
            {
            }
        }

        public class InvalidCredentialsException : Exception
        {
            public InvalidCredentialsException(string message) : base(message)
            {
            }
        }
    }
}