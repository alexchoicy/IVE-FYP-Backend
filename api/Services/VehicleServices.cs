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

namespace api.Services
{
    public interface IVehicleServices
    {
        IEnumerable<VehicleResponseDto> getVehicles(int userID);
        VehicleResponseDto getVehicle(int vehicleID, int userID);
        bool addVehicle(int userID, VehicleRequestDto vehicleRequestDto);
    }
    public class VehicleServices : IVehicleServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;

        public VehicleServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public IEnumerable<VehicleResponseDto> getVehicles(int userID)
        {
            UserVehicles[] uservehicles = normalDataBaseContext.UserVehicles.Where(uv => uv.userID == userID).ToArray();

            List<VehicleResponseDto> vehicleResponseDtos = new List<VehicleResponseDto>();

            foreach (UserVehicles uservehicle in uservehicles)
            {
                vehicleResponseDtos.Add(new VehicleResponseDto
                {
                    vehicleID = uservehicle.vehicleID,
                    vehicleLicense = uservehicle.vehicleLicense,
                    vehicleType = uservehicle.vehicleType,
                    isDisabled = uservehicle.isDisabled
                });
            }

            return vehicleResponseDtos;
        }

        public VehicleResponseDto getVehicle(int vehicleID, int userID)
        {
            UserVehicles? uservehicle = normalDataBaseContext.UserVehicles.FirstOrDefault(uv => uv.vehicleID == vehicleID && uv.userID == userID);

            if (uservehicle == null)
            {
                throw new vehicleNotFoundException("Vehicle not found");
            }

            return new VehicleResponseDto
            {
                vehicleID = uservehicle.vehicleID,
                vehicleLicense = uservehicle.vehicleLicense,
                vehicleType = uservehicle.vehicleType,
                isDisabled = uservehicle.isDisabled
            };
        }

        public bool addVehicle(int userID, VehicleRequestDto vehicleRequestDto)
        {
            string vehicleLicense = vehicleRequestDto.vehicleLicense.ToUpper();
            if (vehicleLicense == "")
            {
                throw new InvalidVehicleLicenseException("Vehicle license cannot be empty");
            }
            UserVehicles? checkVehicle = normalDataBaseContext.UserVehicles.FirstOrDefault(uv => uv.vehicleLicense == vehicleLicense);
            if (checkVehicle != null && !checkVehicle.isDisabled)
            {
                throw new vehicleAlreadyExistsException("Vehicle already exists");
            }

            if (!Enum.IsDefined(typeof(VehicleTypes), vehicleRequestDto.vehicleType))
            {
                throw new InvalidVehicleTypeException("Invalid vehicle type, 1 for Regular, 2 for Electric");
            }

            UserVehicles uservehicle = new UserVehicles
            {
                userID = userID,
                vehicleLicense = vehicleRequestDto.vehicleLicense,
                vehicleType = (VehicleTypes)vehicleRequestDto.vehicleType,
                isDisabled = false,
                createdAt = DateTime.Now
            };

            normalDataBaseContext.UserVehicles.Add(uservehicle);
            normalDataBaseContext.SaveChanges();
            return true;
        }

    }
}