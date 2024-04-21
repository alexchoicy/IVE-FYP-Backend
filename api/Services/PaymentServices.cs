using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Exceptions;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;
using api.utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.Services
{
    public interface IPaymentServices
    {
        IEnumerable<PaymentResponseDto> GetPaymentsByUserId(int userId);
        DetailedPaymentResponseDto GetPayment(int paymentID, int tokenUserID);
        // bool MakePayment(int paymentID, MakePaymentRequestDto makePaymentRequestDto);

        decimal CalculatePrices(ParkingRecords parkingRecord, ParkingLots parkingLot, SpaceType spaceType,
            bool isReservated = false);

        String MakeParkingPaymentAsync(int userID, int sessionID, PaymentMethodType paymentMethodType);
        decimal GetTotalPrices(int userID, int sessionID);
    }

    public class PaymentServices : IPaymentServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;

        public PaymentServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public IEnumerable<PaymentResponseDto> GetPaymentsByUserId(int userId)
        {
            IEnumerable<Payments> payments = normalDataBaseContext.Payments.Where(p => p.userID == userId);
            return payments.Select(p => new PaymentResponseDto
            {
                paymentID = p.paymentID,
                userId = p.userID,
                amount = p.amount,
                paymentType = p.paymentType.ToString(),
                relatedID = p.relatedID,
                paymentMethod = p.paymentMethod.ToString(),
                paymentStatus = p.paymentStatus.ToString(),
                paymentIssuedAt = p.createdAt
            });
        }

        public DetailedPaymentResponseDto GetPayment(int paymentID, int tokenUserID)
        {
            // Payments? payment = normalDataBaseContext.Payments.Find(paymentID);

            // if (payment == null)
            // {
            //     throw new PaymentNotFoundException("Payment not found");
            // }
            //
            // if (payment.userID != tokenUserID)
            // {
            //     throw new TokenInvalidException("Payment not found");
            // }
            //
            // DetailedPaymentResponseDto response = new DetailedPaymentResponseDto
            // {
            //     paymentID = payment.paymentID,
            //     userId = payment.userID,
            //     amount = payment.amount,
            //     paymentType = payment.paymentType.ToString(),
            //     relatedID = payment.relatedID,
            //     paymentMethod = payment.paymentMethod.ToString() ?? "",
            //     paymentStatus = payment.paymentStatus.ToString(),
            //     paymentIssuedAt = payment.createdAt
            // };
            //
            // bool paymentTypeSuccess = Enum.TryParse<PaymentType>(payment.paymentType.ToString(), out PaymentType paymentType);
            //
            // //TODO: other payment types, for now only parking fee
            // switch (paymentType)
            // {
            //     case PaymentType.ParkingFee:
            //         ParkingRecords? parkingRecord = normalDataBaseContext.ParkingRecords.Find(payment.relatedID);
            //
            //
            //         ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.Find(parkingRecord?.lotID);
            //
            //         if (parkingRecord == null)
            //         {
            //             throw new PaymentNotFoundException("Parking record not found");
            //         }
            //
            //         if (parkingLot == null)
            //         {
            //             throw new PaymentNotFoundException("Parking lot not found");
            //         }
            //
            //         response.parkingRecord = new ParkingRecordResponseDto
            //         {
            //             lotID = parkingRecord?.lotID ?? 0,
            //             lotName = parkingLot.name,
            //             spaceType = parkingRecord?.spaceType.ToString() ?? "",
            //             entryTime = parkingRecord?.entryTime ?? TimeLoader.GetTime(),
            //             exitTime = parkingRecord?.exitTime,
            //             vehicleLicense = parkingRecord?.vehicleLicense ?? ""
            //         };
            //
            //         //not exit yet
            //         if (payment.amount != -1)
            //         {
            //             return response;
            //         }
            //         bool spaceTypeSuccess = Enum.TryParse<SpaceType>(parkingRecord.spaceType.ToString(), out SpaceType spaceType);
            //
            //         Reservations? reservation = null;
            //         if (parkingRecord.reservationID != null)
            //         {
            //             reservation = normalDataBaseContext.Reservations.Find(parkingRecord.reservationID);
            //
            //             if (reservation == null)
            //             {
            //                 throw new PaymentNotFoundException("Reservation not found");
            //             }
            //             response.reservation = new ReservationResponseDto
            //             {
            //                 reservationID = reservation.reservationID,
            //                 lotID = reservation.lotID,
            //                 lotName = parkingLot.name,
            //                 vehicleLicense = parkingRecord.vehicleLicense,
            //                 spaceType = parkingRecord.spaceType.ToString(),
            //                 startTime = reservation.startTime,
            //                 endTime = reservation.endTime,
            //                 price = reservation.price,
            //                 reservationStatus = reservation.reservationStatus,
            //                 createdTime = reservation.createdAt,
            //                 cancelledTime = reservation.canceledAt
            //             };
            //         }
            //
            //         decimal totalPrices = CalculatePrices(parkingRecord, parkingLot, spaceType, reservation != null);
            //
            //
            //         Console.WriteLine(totalPrices);
            //         response.amount = totalPrices;
            //         break;
            // }
            return null;
        }


        // //simple success payment
        // public bool MakePayment(int paymentID, MakePaymentRequestDto makePaymentRequestDto)
        // {
        //     bool paymentMethodSuccess =
        //         Enum.TryParse<PaymentMethod>(makePaymentRequestDto.paymentMethod, out PaymentMethod paymentMethod);
        //
        //     if (!paymentMethodSuccess)
        //     {
        //         throw new RequestInvalidException("Payment method must be: App, PaymentMachine");
        //     }
        //
        //     bool paymentMethodTypeSuccess = Enum.TryParse<PaymentMethodType>(makePaymentRequestDto.paymentMethodType,
        //         out PaymentMethodType paymentMethodType);
        //
        //     if (!paymentMethodTypeSuccess)
        //     {
        //         throw new RequestInvalidException(
        //             "Payment method type must be: CreditCard, Cash, DebitCard, ApplePay, GooglePay, SamsungPay");
        //     }
        //
        //     Payments? payment = normalDataBaseContext.Payments.Find(paymentID);
        //
        //     if (payment == null)
        //     {
        //         throw new PaymentNotFoundException("Payment not found");
        //     }
        //
        //     if (payment.paymentStatus != PaymentStatus.Pending)
        //     {
        //         throw new PaymentNotFoundException("Payment is Finished");
        //     }
        //
        //     Reservations? reservation = null;
        //     ParkingRecords? parkingRecord = normalDataBaseContext.ParkingRecords.Find(payment.relatedID);
        //
        //     if (parkingRecord == null)
        //     {
        //         throw new PaymentNotFoundException("Parking record not found");
        //     }
        //
        //     ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.Find(parkingRecord.lotID);
        //
        //     if (parkingLot == null)
        //     {
        //         throw new PaymentNotFoundException("Parking lot not found");
        //     }
        //
        //     SpaceType spaceType = parkingRecord.spaceType;
        //
        //     if (parkingRecord.reservationID != null)
        //     {
        //         reservation = normalDataBaseContext.Reservations.Find(parkingRecord.reservationID);
        //
        //         if (reservation == null)
        //         {
        //             throw new PaymentNotFoundException("Reservation not found");
        //         }
        //     }
        //
        //     decimal totalPrices = CalculatePrices(parkingRecord, parkingLot, spaceType, reservation != null);
        //
        //
        //     payment.amount = totalPrices;
        //     payment.paymentMethod = paymentMethod;
        //     payment.paymentMethodType = paymentMethodType;
        //     payment.paymentStatus = PaymentStatus.Completed;
        //     payment.paymentTime = TimeLoader.GetTime();
        //
        //     normalDataBaseContext.SaveChanges();
        //
        //     return true;
        // }

        public decimal CalculatePrices(ParkingRecords parkingRecord, ParkingLots parkingLot, SpaceType spaceType,
            bool isReservated = false)
        {
            decimal? discount = null;
            if (isReservated)
            {
                discount = parkingLot.reservedDiscount;
            }

            decimal totalPrices = 0;
            switch (spaceType)
            {
                case SpaceType.ELECTRIC:
                    IEnumerable<LotPrices> electriclotPrices =
                        JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.electricSpacePrices);
                    totalPrices = PaymentUtils.CalculateParkingFee(electriclotPrices, parkingRecord.entryTime,
                        parkingRecord.exitTime ?? TimeLoader.GetTime(), discount);
                    break;
                default:
                    IEnumerable<LotPrices> regularlotPrices =
                        JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.regularSpacePrices);
                    totalPrices = PaymentUtils.CalculateParkingFee(regularlotPrices, parkingRecord.entryTime,
                        parkingRecord.exitTime ?? TimeLoader.GetTime(), discount);
                    break;
            }

            return totalPrices;
        }

        public decimal GetTotalPrices(int userID, int sessionID)
        {
            decimal totalAmount = 0;

            ICollection<ParkingRecords> records = normalDataBaseContext.ParkingRecords
                .Where(p => p.sessionID == sessionID)
                .Include(x => x.payment)
                .Include(x => x.reservation)
                .ToList();

            ParkingLots? parkinglot =
                normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == records.First().lotID);
            if (parkinglot == null)
            {
                throw new ParkingLotNotFoundException("Parking lot not found");
            }

            foreach (ParkingRecords record in records)
            {
                IEnumerable<LotPrices>? lotPrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(
                    record.spaceType == SpaceType.REGULAR
                        ? parkinglot.regularSpacePrices
                        : parkinglot.electricSpacePrices);

                if (record.payment.paymentStatus == PaymentStatus.Completed)
                {
                    continue;
                }

                int totalHours = (int)(TimeLoader.GetTime() - record.entryTime).TotalHours;
                int totalMinutes = (int)(TimeLoader.GetTime() - record.entryTime).TotalMinutes % 60;

                if (totalMinutes > 15)
                {
                    totalHours++;
                }

                decimal price;
                if (record.reservation == null)
                {
                    Console.WriteLine("1) No reservation");
                    price = CalculatePriceWithoutReservation(totalHours, lotPrices);
                }
                else if (TimeLoader.GetTime() <= record.reservation.endTime.AddMinutes(30))
                {
                    Console.WriteLine("2) Reservation is not expired");
                    price = CalculatePriceWithReservation(totalHours, lotPrices, parkinglot.reservedDiscount);
                }
                else
                {
                    //int cast to round 
                    int totalReservationHours =
                        (int)(record.reservation.endTime - record.reservation.startTime).TotalHours;
                    int totalHoursAfterReservation = totalHours - totalReservationHours;
                    Console.WriteLine("3) Reservation is expired");
                    Console.WriteLine("Total Reservation Hours: " + totalReservationHours);
                    Console.WriteLine("Total Hours After Reservation: " + totalHoursAfterReservation);
                    price = CalculatePriceWithReservation(totalReservationHours, lotPrices,
                        parkinglot.reservedDiscount);
                    price += CalculatePriceWithoutReservation(totalHoursAfterReservation, lotPrices);
                }

                if (record.exitTime != null)
                {
                    totalAmount += record.payment.amount;
                    continue;
                }

                if (price > record.payment.amount)
                {
                    record.payment.amount = price;
                    record.payment.paymentStatus = PaymentStatus.Pending;
                    normalDataBaseContext.SaveChanges();
                }

                totalAmount += price;
            }

            return totalAmount;
        }


        public string MakeParkingPaymentAsync(int userID, int sessionID, PaymentMethodType paymentMethodType)
        {
            ICollection<ParkingRecords> records = normalDataBaseContext.ParkingRecords
                .Where(p => p.sessionID == sessionID)
                .Include(x => x.payment)
                .Include(x => x.reservation)
                .ToList();

            foreach (ParkingRecords record in records)
            {
                if (record.payment.paymentStatus == PaymentStatus.Completed)
                {
                    continue;
                }

                if (record.reservation != null)
                {
                    record.reservation.reservationStatus = ReservationStatus.COMPLETED;
                    normalDataBaseContext.SaveChanges();
                }
                record.payment.paymentTime = TimeLoader.GetTime();
                record.payment.paymentMethod = PaymentMethod.App;
                record.payment.paymentMethodType = paymentMethodType;
                record.payment.paymentStatus = PaymentStatus.Completed;
                normalDataBaseContext.SaveChanges();
            }

            return "Payment completed";
        }

        private decimal CalculatePriceWithoutReservation(int totalHours, IEnumerable<LotPrices> lotPrices)
        {
            decimal price = 0;

            for (int i = 0; i < totalHours; i++)
            {
                string timeString = (TimeLoader.GetTime() - TimeSpan.FromHours(totalHours - i)).ToString("HH:00");
                decimal hourPrice = lotPrices.FirstOrDefault(lp => lp.time == timeString)?.price ?? 0;
                price += hourPrice;
            }

            return price;
        }

        private decimal CalculatePriceWithReservation(int totalHours, IEnumerable<LotPrices> lotPrices,
            decimal discount)
        {
            decimal price = CalculatePriceWithoutReservation(totalHours, lotPrices);

            return price * (1 - discount);
        }
    }
}