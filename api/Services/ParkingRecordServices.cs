using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Respone;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.Services
{
    public interface IParkingRecordServices
    {
        Task<ICollection<ParkingRecordResponseDtoDetailed>> GetParkingRecordsAsync(int userID, int recordsPerPage, int page);
        Task<ParkingRecordResponseDtoDetailed> GetParkingRecord(int userID, int sessionID);
    }

    public class ParkingRecordServices : IParkingRecordServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public ParkingRecordServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }
        public async Task<ICollection<ParkingRecordResponseDtoDetailed>> GetParkingRecordsAsync(int userID, int recordsPerPage, int page)
        {
            ICollection<ParkingRecordResponseDtoDetailed> parkingRecordsDetailed = [];

            IEnumerable<ParkingRecordSessions> parkingRecordSessions = await normalDataBaseContext.ParkingRecordSessions
                .Where(prs => normalDataBaseContext.UserVehicles.Any(uv => uv.vehicleLicense == prs.vehicleLicense && uv.userID == userID))
                .OrderByDescending(prs => prs.CreatedAt)
                .Include(p => p.parkingLot)
                .Skip(recordsPerPage * (page - 1))
                .Take(recordsPerPage)
                .ToListAsync();

            if (parkingRecordSessions.Count() == 0 || parkingRecordSessions == null)
            {
                return parkingRecordsDetailed;
            }

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

                IEnumerable<ParkingRecords> parkingRecords = await normalDataBaseContext.ParkingRecords
                    .Where(pr => pr.sessionID == parkingRecordSession.sessionID)
                    .Include(p => p.parkingLot)
                    .Include(p => p.reservation)
                    .Include(p => p.payment)
                    .ToListAsync();

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
                    parkingRecordResponseDto.records.Add(parkingRecordResponseDtoDetailedHistory);

                }
                parkingRecordsDetailed.Add(parkingRecordResponseDto);
            }


            return parkingRecordsDetailed;
        }

        public async Task<ParkingRecordResponseDtoDetailed> GetParkingRecord(int userID, int sessionID)
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

            IEnumerable<ParkingRecords> parkingRecords = await normalDataBaseContext.ParkingRecords.Where(pr => pr.sessionID == sessionID)
                .Include(p => p.parkingLot)
                .Include(p => p.reservation)
                .Include(p => p.payment)
                .ToListAsync();

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
                parkingRecordResponseDto.records.Add(parkingRecordResponseDtoDetailedHistory);
            }
            return parkingRecordResponseDto;
        }

    }
}