using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.utils
{
    public static class PaymentUtils
    {
        public static decimal CalculateParkingFee(decimal hourlyRate, DateTime start, DateTime end)
        {
            TimeSpan duration = end - start;
            decimal totalHours = (decimal)duration.TotalHours;
            decimal totalCost = totalHours * hourlyRate;
            return totalCost;
        }
    }
}