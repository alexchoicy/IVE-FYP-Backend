using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class ParkingLotNotFoundException : Exception
    {
        public ParkingLotNotFoundException(string message) : base(message)
        {
        }
    }

    public class ParkingLotPriceTimesInvalidException : Exception
    {
        public ParkingLotPriceTimesInvalidException(string message) : base(message)
        {
        }
    }

    public class ParkingLotPriceTimeInvalidException : Exception
    {
        public ParkingLotPriceTimeInvalidException(string message) : base(message)
        {
        }
    }

    public class ParkingLotPriceInvalidException : Exception
    {
        public ParkingLotPriceInvalidException(string message) : base(message)
        {
        }
    }

}