using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;

namespace api.Services
{
    public interface IReservationServices
    {
        IEnumerable<ReservationResponseDto> getReservationsByUserID(int userID);
        IEnumerable<ReservationResponseDto> getReservationsByLotID(int lotID);
        IEnumerable<ReservationResponseDto> getReservationsByVehicleID(int vehicleID);
        ReservationResponseDto getReservationByID(int reservationID);
        bool createReservation(int userID, CreateReservationRequestDto createReservationRequestDto);
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

        public IEnumerable<ReservationResponseDto> getReservationsByUserID(int userID)
        {
            IEnumerable<ReservationResponseDto> reservations = normalDataBaseContext.Reservations
                .Where(r => r.vehicle.userID == userID)
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
                    price = r.price,
                    reservationStatus = r.reservationStatus,
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt
                });
            return reservations;
        }

        public IEnumerable<ReservationResponseDto> getReservationsByLotID(int lotID)
        {
            IEnumerable<ReservationResponseDto> reservations = normalDataBaseContext.Reservations
                .Where(r => r.lotID == lotID)
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
                    price = r.price,
                    reservationStatus = r.reservationStatus,
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt
                });
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
                    price = r.price,
                    reservationStatus = r.reservationStatus,
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt
                });
            return reservations;
        }

        public ReservationResponseDto getReservationByID(int reservationID)
        {
            ReservationResponseDto? reservation = normalDataBaseContext.Reservations.Where(r => r.reservationID == reservationID)
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
                    price = r.price,
                    reservationStatus = r.reservationStatus,
                    createdTime = r.createdAt,
                    cancelledTime = r.canceledAt
                }).FirstOrDefault();

            if (reservation == null)
            {
                throw new ReservationNotFoundException("Reservation not found");
            }
            return reservation;
        }


        //TODO add price and payment
        public bool createReservation(int userID, CreateReservationRequestDto createReservationRequestDto)
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

            if (reservations.Count() > 0)
            {
                throw new ReservationTimeConflictException("Reservation time conflict");
            }


            for (var hour = roundedStartTime; hour < roundedEndTime; hour = hour.AddHours(1))
            {
                HourlyAvailableSpaces? hourlyAvailableSpaces = normalDataBaseContext.HourlyAvailableSpaces.FirstOrDefault(has => has.lotID == createReservationRequestDto.lotID && has.dateTime == hour);

                if (hourlyAvailableSpaces != null)
                {
                    switch (spaceType)
                    {
                        case Enums.SpaceType.REGULAR:
                            if (hourlyAvailableSpaces.regularSpaceCount <= 0)
                            {
                                throw new NoAvailableSpacesException("No available spaces at " + hour.ToString("yyyy-MM-dd HH:mm:ss") + " for regular spaces");
                            }
                            hourlyAvailableSpaces.regularSpaceCount--;
                            break;
                        case Enums.SpaceType.ELECTRIC:
                            if (hourlyAvailableSpaces.electricSpaceCount <= 0)
                            {
                                throw new NoAvailableSpacesException("No available spaces at " + hour.ToString("yyyy-MM-dd HH:mm:ss") + " for electric spaces");
                            }
                            hourlyAvailableSpaces.electricSpaceCount--;
                            break;
                    }
                }
                else
                {
                    HourlyAvailableSpaces newHourlyAvailableSpaces = hourlyAvaiableSpaceServices.CreateHourlyAvaiableSpace(parkingLot, hour);
                    switch (spaceType)
                    {
                        case Enums.SpaceType.REGULAR:
                            newHourlyAvailableSpaces.regularSpaceCount--;
                            break;
                        case Enums.SpaceType.ELECTRIC:
                            newHourlyAvailableSpaces.electricSpaceCount--;
                            break;
                    }
                    normalDataBaseContext.HourlyAvailableSpaces.Add(newHourlyAvailableSpaces);
                }
            }

            int reservationCount = normalDataBaseContext.Reservations
                .Count(r => r.vehicleID == createReservationRequestDto.vehicleID &&
                            r.startTime.Date == roundedStartTime.Date);

            if (reservationCount >= 10)
            {
                throw new ReservationLimitExceededException("Reservation 10 limit exceeded");
            }

            Reservations newReservation = new Reservations
            {
                vehicleID = createReservationRequestDto.vehicleID,
                lotID = createReservationRequestDto.lotID,
                startTime = roundedStartTime,
                endTime = roundedEndTime,
                spaceType = spaceType,
                reservationStatus = Enums.ReservationStatus.PENDING,
                createdAt = DateTime.Now
            };
            //TODO a method to generate a payment
            normalDataBaseContext.Reservations.Add(newReservation);
            normalDataBaseContext.SaveChanges();
            return true;
        }
    }
}