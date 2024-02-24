using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class InvalidEmailException : BaseCustomExceptions
    {
        public InvalidEmailException(string message) : base(message)
        {
        }
    }

    public class InvalidPhoneNumberException : BaseCustomExceptions
    {
        public InvalidPhoneNumberException(string message) : base(message)
        {
        }
    }

}