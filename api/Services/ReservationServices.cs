using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Exceptions;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public interface IReservationServices
    {
        Task<IEnumerable<ReservationResponseDto>> getReservationsByUserID(int userID);
        Task<IEnumerable<ReservationResponseDto>> getReservationsByLotID(int lotID);
        IEnumerable<ReservationResponseDto> getReservationsByVehicleID(int vehicleID);
        ReservationResponseDto getReservationByID(int reservationID);
        Task<bool> createReservation(int userID, CreateReservationRequestDto createReservationRequestDto);
        string MakeReservationPayment(int id, PaymentMethodType paymentMethodType);
        bool cancelReservation(int reservationID);
        bool editReservation(int reservationID, EditReservationRequestDto editReservationRequestDto);
    }
    public class ReservationServices : IReservationServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;

        private readonly IHourlyAvaiableSpaceServices hourlyAvaiableSpaceServices;
        public ReservationServices(NormalDataBaseContext normalDataBaseContext, IHourlyAvaiableSpaceServices hourlyAvaiableSpaceServices)
        {
            this.normalDataBaseContext = normalDataBaseContext;
            this.hourlyAvaiableSpaceServices = hourlyAvaiableSpaceServices;
        }

        public async Task<IEnumerable<ReservationResponseDto>> getReservationsByUserID(int userID)
        {
            Console.WriteLine(userID);
            IEnumerable<ReservationResponseDto> reservations = await normalDataBaseContext.Reservations
                .Where(r => r.vehicle.userID == userID)
                .Include(r => r.lot)
                .Select(r => new ReservationResponseDto
                {
                    reservationID = r.reservationID,
                    lotID = r.lotID,
                    lotName = r.lot.name,
                    spaceType = r.spaceType.ToString(),
                    vehicleID = r.vehicleID,
                    vehicleLicense = r.vehicle.vehicleLicense,
                    startTime = r.startTime,
                    endTime = r.endTime,
                    paymentID = r.payment.paymentID,
                    payment = new PaymentResponseDto
                    {
                        paymentMethod = r.payment.paymentMethod.ToString(),
                        amount = r.payment.amount,
                        paymentID = r.payment.paymentID,
                        paymentIssuedAt = r.payment.createdAt,
                        paymentStatus = r.payment.paymentStatus.ToString(),
                        paymentType = r.payment.paymentType.ToString(),
                        relatedID = r.payment.relatedID,
                        userId = r.payment.userID,
                        paymentTime = r.payment.paymentTime
                    },
                    reservationStatus = r.reservationStatus,
                    reservationStatusString = r.reservationStatus.ToString(),
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt,
                    status = r.reservationStatus.ToString()
                }).ToListAsync();

            return reservations;
        }

        public async Task<IEnumerable<ReservationResponseDto>> getReservationsByLotID(int lotID)
        {
            Console.WriteLine("lot" + lotID);
            IEnumerable<ReservationResponseDto> reservations = await normalDataBaseContext.Reservations
                .Where(r => r.lotID == lotID)
                .Include(r => r.lot)
                .OrderByDescending(r => r.createdAt)
                .ThenBy(r => r.reservationStatus == ReservationStatus.PENDING)
                .Select(r => new ReservationResponseDto
                {
                    reservationID = r.reservationID,
                    lotID = r.lotID,
                    lotName = r.lot.name,
                    spaceType = r.spaceType.ToString(),
                    vehicleID = r.vehicleID,
                    vehicleLicense = r.vehicle.vehicleLicense,
                    startTime = r.startTime,
                    endTime = r.endTime,
                    paymentID = r.payment.paymentID,
                    payment = new PaymentResponseDto
                    {
                        paymentMethod = r.payment.paymentMethod.ToString(),
                        amount = r.payment.amount,
                        paymentID = r.payment.paymentID,
                        paymentIssuedAt = r.payment.createdAt,
                        paymentStatus = r.payment.paymentStatus.ToString(),
                        paymentType = r.payment.paymentType.ToString(),
                        relatedID = r.payment.relatedID,
                        userId = r.payment.userID,
                        paymentTime = r.payment.paymentTime
                    },
                    reservationStatus = r.reservationStatus,
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt,
                    status = r.reservationStatus.ToString()
                }).ToListAsync<ReservationResponseDto>();
            return reservations;
        }

        public IEnumerable<ReservationResponseDto> getReservationsByVehicleID(int vehicleID)
        {
            IEnumerable<ReservationResponseDto> reservations = normalDataBaseContext.Reservations
                .Where(r => r.vehicleID == vehicleID)
                .Select(r => new ReservationResponseDto
                {
                    reservationID = r.reservationID,
                    lotID = r.lotID,
                    lotName = r.lot.name,
                    spaceType = r.spaceType.ToString(),
                    vehicleID = r.vehicleID,
                    vehicleLicense = r.vehicle.vehicleLicense,
                    startTime = r.startTime,
                    endTime = r.endTime,
                    payment = new PaymentResponseDto
                    {
                        paymentMethod = r.payment.paymentMethod.ToString(),
                        amount = r.payment.amount,
                        paymentID = r.payment.paymentID,
                        paymentIssuedAt = r.payment.createdAt,
                        paymentStatus = r.payment.paymentStatus.ToString(),
                        paymentType = r.payment.paymentType.ToString(),
                        relatedID = r.payment.relatedID,
                        userId = r.payment.userID,
                        paymentTime = r.payment.paymentTime
                    },
                    reservationStatus = r.reservationStatus,
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt
                });
            return reservations;
        }

        public ReservationResponseDto getReservationByID(int reservationID)
        {
            ReservationResponseDto? reservation = normalDataBaseContext.Reservations.Where(r => r.reservationID == reservationID)
                .Include(r => r.payment)
                .Select(r => new ReservationResponseDto
                {
                    reservationID = r.reservationID,
                    lotID = r.lotID,
                    lotName = r.lot.name,
                    spaceType = r.spaceType.ToString(),
                    vehicleID = r.vehicleID,
                    vehicleLicense = r.vehicle.vehicleLicense,
                    startTime = r.startTime,
                    endTime = r.endTime,
                    paymentID = r.paymentID,
                    payment = new PaymentResponseDto
                    {
                        paymentMethod = r.payment.paymentMethod.ToString(),
                        amount = r.payment.amount,
                        paymentID = r.payment.paymentID,
                        paymentIssuedAt = r.payment.createdAt,
                        paymentStatus = r.payment.paymentStatus.ToString(),
                        paymentType = r.payment.paymentType.ToString(),
                        paymentMethodType = r.payment.paymentMethodType.ToString(),
                        relatedID = r.payment.relatedID,
                        userId = r.payment.userID,
                        paymentTime = r.payment.paymentTime
                    },
                    reservationStatus = r.reservationStatus,
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt,
                    status = r.reservationStatus.ToString()
                }).FirstOrDefault();

            if (reservation == null)
            {
                throw new ReservationNotFoundException("Reservation not found");
            }
            return reservation;
        }


        public async Task<bool> createReservation(int userID, CreateReservationRequestDto createReservationRequestDto)
        {
            UserVehicles? uservehicle = normalDataBaseContext.UserVehicles.FirstOrDefault(uv => uv.vehicleID == createReservationRequestDto.vehicleID && uv.userID == userID);

            if (uservehicle == null)
            {
                throw new vehicleNotFoundException("Vehicle not found");
            }

            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(pl => pl.lotID == createReservationRequestDto.lotID);

            if (parkingLot == null)
            {
                throw new ParkingLotNotFoundException("Parking lot not found");
            }

            DateTime roundedStartTime = new DateTime(createReservationRequestDto.startTime.Year, createReservationRequestDto.startTime.Month, createReservationRequestDto.startTime.Day, createReservationRequestDto.startTime.Hour, 0, 0);
            DateTime roundedEndTime = new DateTime(createReservationRequestDto.endTime.Year, createReservationRequestDto.endTime.Month, createReservationRequestDto.endTime.Day, createReservationRequestDto.endTime.Hour, 0, 0);


            if (roundedStartTime < DateTime.Now || roundedEndTime < DateTime.Now || roundedStartTime > roundedEndTime)
            {
                throw new InvalidReservationTimeException("Invalid reservation time");
            }

            if ((roundedEndTime - roundedStartTime).TotalHours > parkingLot.maxReservationHours)
            {
                throw new InvalidReservationTimeException("Reservation time exceeds the maximum allowed time");
            }

            if (createReservationRequestDto.startTime.Minute != 0 || createReservationRequestDto.endTime.Minute != 0)
            {
                throw new InvalidReservationTimeException("Reservation start and end time must be at the top of the hour");
            }

            bool success = Enum.TryParse(createReservationRequestDto.spaceType.ToString(), out Enums.SpaceType spaceType);

            if (!success)
            {
                throw new InvalidSpaceTypeException("Invalid space type, must be REGULAR or ELECTRIC");
            }

            if (spaceType == Enums.SpaceType.ELECTRIC && uservehicle.vehicleType != Enums.VehicleTypes.ELECTRIC)
            {
                throw new InvalidSpaceTypeException("Incorrect vehicle type for electric space type");
            }



            IEnumerable<Reservations> reservations = normalDataBaseContext.Reservations
                .Where(r => r.lotID == createReservationRequestDto.lotID &&
                            r.vehicleID == createReservationRequestDto.vehicleID &&
                            ((r.startTime <= roundedStartTime && r.endTime > roundedStartTime) ||
                            (r.startTime < roundedEndTime && r.endTime >= roundedEndTime) ||
                            (r.startTime >= roundedStartTime && r.endTime <= roundedEndTime)))
                .ToList();

            if (reservations.Count() > 0 && reservations.Any(r => r.reservationStatus != ReservationStatus.CANCELLED))
            {
                throw new ReservationTimeConflictException("Reservation time conflict");
            }


            HourlyReservationCount? hourlyAvailableSpaces = normalDataBaseContext.HourlyReservationCounts.FirstOrDefault(has => has.lotID == createReservationRequestDto.lotID && has.dateTime == roundedStartTime);

            if (hourlyAvailableSpaces != null)
            {
                switch (spaceType)
                {
                    case Enums.SpaceType.REGULAR:
                        if (hourlyAvailableSpaces.regularSpaceCount == parkingLot.regularSpaces)
                        {
                            throw new NoAvailableSpacesException("No available spaces at " + roundedStartTime.ToString("yyyy-MM-dd HH:mm:ss") + " for regular spaces");
                        }
                        hourlyAvailableSpaces.regularSpaceCount++;
                        break;
                    case Enums.SpaceType.ELECTRIC:
                        if (hourlyAvailableSpaces.electricSpaceCount == parkingLot.electricSpaces)
                        {
                            throw new NoAvailableSpacesException("No available spaces at " + roundedStartTime.ToString("yyyy-MM-dd HH:mm:ss") + " for electric spaces");
                        }
                        hourlyAvailableSpaces.electricSpaceCount++;
                        break;
                }
            }
            else
            {
                HourlyReservationCount newHourlyAvailableSpaces = hourlyAvaiableSpaceServices.CreateHourlyAvaiableSpace(parkingLot, roundedStartTime);
                switch (spaceType)
                {
                    case Enums.SpaceType.REGULAR:
                        newHourlyAvailableSpaces.regularSpaceCount++;
                        break;
                    case Enums.SpaceType.ELECTRIC:
                        newHourlyAvailableSpaces.electricSpaceCount++;
                        break;
                }
                normalDataBaseContext.HourlyReservationCounts.Add(newHourlyAvailableSpaces);
            }

            int reservationCount = normalDataBaseContext.Reservations
                .Count(r => r.vehicleID == createReservationRequestDto.vehicleID &&
                            r.startTime.Date == roundedStartTime.Date);

            if (reservationCount >= 10)
            {
                throw new ReservationLimitExceededException("Reservation 10 limit exceeded");
            }

            Payments payment = new Payments
            {
                paymentType = PaymentType.Reservation,
                userID = userID,
                amount = 10,
                paymentStatus = PaymentStatus.Generated,
            };

            await normalDataBaseContext.Payments.AddAsync(payment);
            await normalDataBaseContext.SaveChangesAsync();

            Reservations newReservation = new Reservations
            {
                vehicleID = createReservationRequestDto.vehicleID,
                lotID = createReservationRequestDto.lotID,
                startTime = roundedStartTime,
                endTime = roundedEndTime,
                spaceType = spaceType,
                reservationStatus = Enums.ReservationStatus.PENDING,
                createdAt = DateTime.Now,
                paymentID = payment.paymentID
            };
            //TODO a method to generate a payment
            normalDataBaseContext.Reservations.Add(newReservation);
            normalDataBaseContext.SaveChanges();

            payment.relatedID = newReservation.reservationID;
            normalDataBaseContext.Payments.Update(payment);
            normalDataBaseContext.SaveChanges();

            return true;
        }

        public string MakeReservationPayment(int id, PaymentMethodType paymentMethodType)
        {
            Reservations reservation = normalDataBaseContext.Reservations.Include(x => x.payment)
                .FirstOrDefault(r => r.reservationID == id);

            if (reservation == null)
            {
                return "nothing";
            }
            
            reservation.payment.paymentStatus = PaymentStatus.Completed;
            reservation.payment.paymentTime = DateTime.Now;
            reservation.payment.paymentMethod = PaymentMethod.App;
            reservation.payment.paymentMethodType = paymentMethodType;
            reservation.reservationStatus = ReservationStatus.PAID;
            normalDataBaseContext.Reservations.Update(reservation);
            normalDataBaseContext.SaveChanges();
            
            return "success";
        }
        
        public  bool cancelReservation(int reservationID)
        {
            Reservations? reservation = normalDataBaseContext.Reservations
                .Include(r => r.payment)
                .FirstOrDefault(r => r.reservationID == reservationID);

            if (reservation == null)
            {
                throw new ReservationNotFoundException("Reservation not found");
            }

            if (reservation.reservationStatus == ReservationStatus.ACTIVE || reservation.reservationStatus == ReservationStatus.COMPLETED)
            {
                throw new ReservationStatusConflictException("The reservation is active or completed");
            }
            
            if (reservation.reservationStatus == ReservationStatus.CANCELLED)
            {
                throw new ReservationStatusConflictException("Reservation already cancelled");
            }

            if (reservation.reservationStatus == ReservationStatus.PAID)
            {
                throw new ReservationStatusConflictException("Reservation already paid");
            }

            if (reservation.reservationStatus == ReservationStatus.PENDING)
            {
                if (reservation.startTime.AddHours(1) < DateTime.Now)
                {
                    throw new ReservationTimeConflictException("Reservation time conflict");
                }
            }

            reservation.reservationStatus = ReservationStatus.CANCELLED;
            reservation.canceledAt = DateTime.Now;
            normalDataBaseContext.Reservations.Update(reservation);
            normalDataBaseContext.SaveChanges();

            return true;
        }

        public bool editReservation(int reservationID, EditReservationRequestDto editReservationRequestDto)
        {
            Reservations? reservation = normalDataBaseContext.Reservations
                .Include(r => r.payment)
                .Include(r => r.lot)
                .Include(r => r.vehicle)
                .FirstOrDefault(r => r.reservationID == reservationID);

            if (reservation == null)
            {
                throw new ReservationNotFoundException("Reservation not found");
            }
            
            if (reservation.reservationStatus == ReservationStatus.CANCELLED)
            {
                throw new ReservationStatusConflictException("Reservation already cancelled");
            }
            
            // if (reservation.reservationStatus == ReservationStatus.PAID)
            // {
            //     throw new ReservationStatusConflictException("Reservation already paid");
            // }
            
            if (reservation.reservationStatus == ReservationStatus.PENDING)
            {
                if (reservation.startTime.AddHours(1) < DateTime.Now)
                {
                    throw new ReservationTimeConflictException("Reservation time conflict");
                }
            }
            
            DateTime roundedStartTime = new DateTime(editReservationRequestDto.startTime.Year, editReservationRequestDto.startTime.Month, editReservationRequestDto.startTime.Day, editReservationRequestDto.startTime.Hour, 0, 0);
            DateTime roundedEndTime = new DateTime(editReservationRequestDto.endTime.Year, editReservationRequestDto.endTime.Month, editReservationRequestDto.endTime.Day, editReservationRequestDto.endTime.Hour, 0, 0);
            
            if (roundedStartTime < DateTime.Now || roundedEndTime < DateTime.Now || roundedStartTime > roundedEndTime)
            {
                throw new InvalidReservationTimeException("Invalid reservation time");
            }
            
            if ((roundedEndTime - roundedStartTime).TotalHours > reservation.lot.maxReservationHours)
            {
                throw new InvalidReservationTimeException("Reservation time exceeds the maximum allowed time");
            }
            
            if (editReservationRequestDto.startTime.Minute != 0 || editReservationRequestDto.endTime.Minute != 0)
            {
                throw new InvalidReservationTimeException("Reservation start and end time must be at the top of the hour");
            }
            
            bool success = Enum.TryParse(editReservationRequestDto.spaceType.ToString(), out Enums.SpaceType spaceType);
            
            if (!success)
            {
                throw new InvalidSpaceTypeException("Invalid space type, must be REGULAR or ELECTRIC");
            }
            
            if (spaceType == Enums.SpaceType.ELECTRIC && reservation.vehicle.vehicleType != Enums.VehicleTypes.ELECTRIC)
            {
                throw new InvalidSpaceTypeException("Incorrect vehicle type for electric space type");
            }
            
            
            
            HourlyReservationCount oldsession = normalDataBaseContext.HourlyReservationCounts.FirstOrDefault(p => p.lotID == reservation.lotID && p.dateTime == reservation.startTime);
            
            if (oldsession != null)
            {
                switch (reservation.spaceType)
                {
                    case Enums.SpaceType.REGULAR:
                        oldsession.regularSpaceCount--;
                        break;
                    case Enums.SpaceType.ELECTRIC:
                        oldsession.electricSpaceCount--;
                        break;
                }
            }
            
            HourlyReservationCount newsession = normalDataBaseContext.HourlyReservationCounts.FirstOrDefault(p => p.lotID == reservation.lotID && p.dateTime == roundedStartTime);

            if (newsession != null)
            {
                switch (spaceType)
                {
                    case Enums.SpaceType.REGULAR:
                        if (newsession.regularSpaceCount == reservation.lot.regularSpaces)
                        {
                            throw new NoAvailableSpacesException("No available spaces at " + roundedStartTime.ToString("yyyy-MM-dd HH:mm:ss") + " for regular spaces");
                        }
                        newsession.regularSpaceCount++;
                        break;
                    case Enums.SpaceType.ELECTRIC:
                        if (newsession.electricSpaceCount == reservation.lot.electricSpaces)
                        {
                            throw new NoAvailableSpacesException("No available spaces at " + roundedStartTime.ToString("yyyy-MM-dd HH:mm:ss") + " for electric spaces");
                        }
                        newsession.electricSpaceCount++;
                        break;
                }
                normalDataBaseContext.HourlyReservationCounts.Update(newsession);
            }
            else
            {
                ParkingLots parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(pl => pl.lotID == reservation.lotID);
                
                HourlyReservationCount newHourlyAvailableSpaces = hourlyAvaiableSpaceServices.CreateHourlyAvaiableSpace(parkingLot, roundedStartTime);
                switch (spaceType)
                {
                    case Enums.SpaceType.REGULAR:
                        newHourlyAvailableSpaces.regularSpaceCount++;
                        break;
                    case Enums.SpaceType.ELECTRIC:
                        newHourlyAvailableSpaces.electricSpaceCount++;
                        break;
                }
                normalDataBaseContext.HourlyReservationCounts.Add(newHourlyAvailableSpaces);
            }
            
            reservation.spaceType = spaceType;
            reservation.startTime = roundedStartTime;
            reservation.endTime = roundedEndTime;
            normalDataBaseContext.Reservations.Update(reservation);
            normalDataBaseContext.SaveChanges();

            return true;
        }
    }
}