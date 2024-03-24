using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Exceptions
{
    public class ParkingRecordNotFoundException : BaseCustomExceptions
    {
        public ParkingRecordNotFoundException(string message) : base(message)
        {
        }
    }
}