using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entity.NormalDB;

namespace api.utils
{
    public static class PaymentUtils
    {

        public static decimal CalculateParkingFee(IEnumerable<LotPrices> lotPrices, DateTime start, DateTime end)
        {
            decimal total = 0;
            var hours = (int)(end - start).TotalHours;

            for (int i = 0; i < hours; i++)
            {
                var currentTime = start.AddHours(i).TimeOfDay;
                decimal price = lotPrices.FirstOrDefault(p => TimeSpan.Parse(p.time) <= currentTime)?.price ?? 0;
                total += price;
            }

            return total;
        }

    }
}