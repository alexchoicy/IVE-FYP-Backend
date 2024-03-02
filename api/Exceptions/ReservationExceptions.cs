using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class InvalidReservationTimeException : BaseCustomExceptions
    {
        public InvalidReservationTimeException(string message) : base(message)
        {
        }
    }

    public class NoAvailableSpacesException : BaseCustomExceptions
    {
        public NoAvailableSpacesException(string message) : base(message)
        {
        }
    }

    public class InvalidSpaceTypeException : BaseCustomExceptions
    {
        public InvalidSpaceTypeException(string message) : base(message)
        {
        }
    }

    public class ReservationLimitExceededException : BaseCustomExceptions
    {
        public ReservationLimitExceededException(string message) : base(message)
        {
        }
    }

    public class ReservationNotFoundException : BaseCustomExceptions
    {
        public ReservationNotFoundException(string message) : base(message)
        {
        }
    }
}