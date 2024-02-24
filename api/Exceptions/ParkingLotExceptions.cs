using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class ParkingLotNotFoundException : BaseCustomExceptions
    {
        public ParkingLotNotFoundException(string message) : base(message)
        {
        }
    }

    public class ParkingLotPriceTimesInvalidException : BaseCustomExceptions
    {
        public ParkingLotPriceTimesInvalidException(string message) : base(message)
        {
        }
    }

    public class ParkingLotPriceTimeInvalidException : BaseCustomExceptions
    {
        public ParkingLotPriceTimeInvalidException(string message) : base(message)
        {
        }
    }

    public class ParkingLotPriceInvalidException : BaseCustomExceptions
    {
        public ParkingLotPriceInvalidException(string message) : base(message)
        {
        }
    }

}