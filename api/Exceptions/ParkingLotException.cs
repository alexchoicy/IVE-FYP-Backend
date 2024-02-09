using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class ParkingLotIDNotExistException : Exception
    {
        public ParkingLotIDNotExistException(string message) : base(message)
        {
        }
    }
}