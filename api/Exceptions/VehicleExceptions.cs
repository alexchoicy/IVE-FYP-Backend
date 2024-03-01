using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class vehicleNotFoundException : BaseCustomExceptions
    {
        public vehicleNotFoundException(string message) : base(message)
        {
        }
    }

    public class vehicleAlreadyExistsException : BaseCustomExceptions
    {
        public vehicleAlreadyExistsException(string message) : base(message)
        {
        }
    }
}