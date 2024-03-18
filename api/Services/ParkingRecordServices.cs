using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Respone;

namespace api.Services
{
    public interface IParkingRecordServices
    {
        IEnumerable<ParkingRecordResponseDtoDetailed> GetParkingRecords(int userID, int recordsPerPage, int page);
        ParkingRecordResponseDtoDetailed GetParkingRecord(int userID, int sessionID);
    }

    public class ParkingRecordServices : IParkingRecordServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public ParkingRecordServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }
        public IEnumerable<ParkingRecordResponseDtoDetailed> GetParkingRecords(int userID, int recordsPerPage, int page)
        {
            IEnumerable<ParkingRecordResponseDtoDetailed> parkingRecordsDetailed = [];

            IEnumerable<UserVehicles> userVehicles = normalDataBaseContext.UserVehicles.Where(uv => uv.userID == userID);

            if (userVehicles.Count() == 0)
            {
                return parkingRecordsDetailed;
            }

            IEnumerable<ParkingRecordSessions> parkingRecordSessions =
                normalDataBaseContext.ParkingRecordSessions.Where(prs => userVehicles.Any(uv => uv.vehicleLicense == prs.vehicleLicense))
                .OrderByDescending(prs => prs.CreatedAt)
                .Skip(recordsPerPage * (page - 1))
                .Take(recordsPerPage);

            foreach (ParkingRecordSessions parkingRecordSession in parkingRecordSessions)
            {
                ParkingRecordResponseDtoDetailed parkingRecordResponseDto = new ParkingRecordResponseDtoDetailed
                {
                    sessionID = parkingRecordSession.sessionID,
                    lotID = parkingRecordSession.lotID,
                    lotName = parkingRecordSession.parkingLot.name,
                    vehicleLicense = parkingRecordSession.vehicleLicense,
                    totalPrice = parkingRecordSession.totalPrice,
                    records = []
                };

                IEnumerable<ParkingRecords> parkingRecords = normalDataBaseContext.ParkingRecords.Where(pr => pr.sessionID == parkingRecordSession.sessionID);
                foreach (ParkingRecords parkingRecord in parkingRecords)
                {
                    ParkingRecordResponseDtoDetailedHistory parkingRecordResponseDtoDetailedHistory = new ParkingRecordResponseDtoDetailedHistory
                    {
                        parkingRecordID = parkingRecord.parkingRecordID,
                        entryTime = parkingRecord.entryTime,
                        exitTime = parkingRecord.exitTime ?? null,
                        period = ((parkingRecord.exitTime ?? DateTime.Now) - parkingRecord.entryTime).TotalMinutes,
                        reservation = parkingRecord.reservationID != null ? new ReservationResponseDto
                        {
                            reservationID = (int)parkingRecord.reservationID,
                            vehicleLicense = parkingRecord.vehicleLicense,
                            lotID = parkingRecord.parkingLot.lotID,
                            spaceType = parkingRecord.reservation.spaceType.ToString(),
                            startTime = parkingRecord.reservation.startTime,
                            endTime = parkingRecord.reservation.endTime,
                            reservationStatus = parkingRecord.reservation.reservationStatus,
                            lotName = "",
                        } : null,
                        price = parkingRecord.payment.amount,
                        spaceType = parkingRecord.spaceType.ToString(),
                        paymentStatus = parkingRecord.payment.paymentStatus.ToString()
                    };
                    parkingRecordResponseDto.records.Append(parkingRecordResponseDtoDetailedHistory);

                }
                parkingRecordsDetailed.Append(parkingRecordResponseDto);
            }


            return parkingRecordsDetailed;
        }

        public ParkingRecordResponseDtoDetailed GetParkingRecord(int userID, int sessionID)
        {
            ParkingRecordSessions? parkingRecordSession = normalDataBaseContext.ParkingRecordSessions.FirstOrDefault(prs => prs.sessionID == sessionID);

            if (parkingRecordSession == null)
            {
                return null;
            }

            IEnumerable<UserVehicles> userVehicles = normalDataBaseContext.UserVehicles.Where(uv => uv.userID == userID);
            if (!userVehicles.Any(uv => uv.vehicleLicense == parkingRecordSession.vehicleLicense))
            {
                return null;
            }

            IEnumerable<ParkingRecords> parkingRecords = normalDataBaseContext.ParkingRecords.Where(pr => pr.sessionID == sessionID);

            ParkingRecordResponseDtoDetailed parkingRecordResponseDto = new ParkingRecordResponseDtoDetailed
            {
                sessionID = parkingRecordSession.sessionID,
                lotID = parkingRecordSession.lotID,
                lotName = parkingRecordSession.parkingLot.name,
                vehicleLicense = parkingRecordSession.vehicleLicense,
                totalPrice = parkingRecordSession.totalPrice,
                records = []
            };

            foreach (ParkingRecords parkingRecord in parkingRecords)
            {
                ParkingRecordResponseDtoDetailedHistory parkingRecordResponseDtoDetailedHistory = new ParkingRecordResponseDtoDetailedHistory
                {
                    parkingRecordID = parkingRecord.parkingRecordID,
                    entryTime = parkingRecord.entryTime,
                    exitTime = parkingRecord.exitTime ?? null,
                    period = ((parkingRecord.exitTime ?? DateTime.Now) - parkingRecord.entryTime).TotalMinutes,
                    reservation = parkingRecord.reservationID != null ? new ReservationResponseDto
                    {
                        reservationID = (int)parkingRecord.reservationID,
                        vehicleLicense = parkingRecord.vehicleLicense,
                        lotID = parkingRecord.parkingLot.lotID,
                        spaceType = parkingRecord.reservation.spaceType.ToString(),
                        startTime = parkingRecord.reservation.startTime,
                        endTime = parkingRecord.reservation.endTime,
                        reservationStatus = parkingRecord.reservation.reservationStatus,
                        lotName = "",
                    } : null,
                    price = parkingRecord.payment.amount,
                    spaceType = parkingRecord.spaceType.ToString(),
                    paymentStatus = parkingRecord.payment.paymentStatus.ToString()
                };
                parkingRecordResponseDto.records.Append(parkingRecordResponseDtoDetailedHistory);
            }
            return parkingRecordResponseDto;
        }

    }
}