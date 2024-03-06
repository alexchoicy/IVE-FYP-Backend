CREATE TABLE `Users` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Salt` varchar(255) NOT NULL,
  `PhoneNumber` varchar(20) DEFAULT NULL,
  `FirstName` varchar(50) DEFAULT NULL,
  `LastName` varchar(50) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isActive` tinyint(1) NOT NULL DEFAULT '1',
  `isLocked` tinyint(1) NOT NULL DEFAULT '0',
  `LoginAttempts` int NOT NULL DEFAULT '0',
  `LockUntil` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `UserName` (`Username`),
  UNIQUE KEY `PhoneNumber` (`PhoneNumber`),
  UNIQUE KEY `Email` (`Email`)
);

CREATE TABLE `UserVehicles` (
    `VehicleID` int NOT NULL AUTO_INCREMENT,
    `UserID` int NOT NULL,
    `vehicleLicense` varchar(50) NOT NULL,
    `vehicleType` varchar(50) NOT NULL,
    `IsDisabled` boolean NOT NULL DEFAULT '0',
    `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (`VehicleID`),
    FOREIGN KEY (`UserID`) REFERENCES `Users`(`UserID`)
);

CREATE TABLE `ParkingLots` (
    `LotID` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) NOT NULL,
    `Address` varchar(255) NOT NULL,
    `Latitude` decimal(10, 8) NOT NULL,
    `Longitude` decimal(11, 8) NOT NULL,
    `TotalSpaces` int NOT NULL,
    `RegularPlanSpaces` int NOT NULL DEFAULT '0',
    `ElectricPlanSpaces` int NOT NULL DEFAULT '0',
    `RegularSpaces` int NOT NULL DEFAULT '0',
    `ElectricSpaces` int NOT NULL DEFAULT '0',
    `ReservableOnlyRegularSpaces` int NOT NULL,
    `ReservableOnlyElectricSpaces` int NOT NULL,
    `WalkinReservedRatio` decimal(10, 2) NOT NULL,
    `ReservedDiscount` decimal(10, 2) NOT NULL,
    `MinReservationWindowHours` int NOT NULL,
    `MaxReservationHours` int NOT NULL,
    `RegularSpacePrices` json NOT NULL,
    `ElectricSpacePrices` json NOT NULL,

    PRIMARY KEY (`LotID`)
);

CREATE TABLE `HourlyAvailableSpaces` (
    `RecordID` int NOT NULL AUTO_INCREMENT,
    `LotID` int NOT NULL,
    `DateTime` timestamp NOT NULL,
    `RegularSpaceCount` int NOT NULL,
    `ElectricSpaceCount` int NOT NULL,

    PRIMARY KEY (`RecordID`),
    FOREIGN KEY (`LotID`) REFERENCES `ParkingLots`(`LotID`)
);

CREATE TABLE `ParkingPlans` (
    `PlanID` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) NOT NULL,
    `PlanType` varchar(50) NOT NULL,
    `Description` varchar(255) NOT NULL,
    `Price` decimal(10, 2) NOT NULL,
    `DurationMonths` int NOT NULL,
    PRIMARY KEY (`PlanID`)
);

CREATE TABLE `ParkingSpaces` (
    `SpaceID` int NOT NULL AUTO_INCREMENT,
    `LotID` int NOT NULL,
    `FloorLevel` int NOT NULL,
    `SpaceNumber` int NOT NULL,
    `SpaceStatus` varchar(50) NOT NULL,
    `SpaceType` varchar(50) NOT NULL,
    `CurrentPlanID` int DEFAULT NULL,
    `PlanEnabled` boolean NOT NULL DEFAULT '0',

    PRIMARY KEY (`SpaceID`),
    FOREIGN KEY (`LotID`) REFERENCES `ParkingLots`(`LotID`),
    FOREIGN KEY (`CurrentPlanID`) REFERENCES `ParkingPlans`(`PlanID`)
);

CREATE TABLE `ParkingSpacePlans` (
    `SpacePlanID` int NOT NULL AUTO_INCREMENT,
    `SpaceID` int NOT NULL,
    `PlanID` int NOT NULL,
    `UserID` int NOT NULL,
    `ParkingSpacePlanStatus` varchar(50) NOT NULL,
    `StartTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `EndTime` timestamp,
    
    `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (`SpacePlanID`),
    FOREIGN KEY (`SpaceID`) REFERENCES `ParkingSpaces`(`SpaceID`),
    FOREIGN KEY (`PlanID`) REFERENCES `ParkingPlans`(`PlanID`),
    FOREIGN KEY (`UserID`) REFERENCES `Users`(`UserID`)
);



CREATE TABLE `Reservations` (
    `ReservationID` int NOT NULL AUTO_INCREMENT,
    `VehicleID` int NOT NULL,
    `LotID` int NOT NULL,
    `StartTime` timestamp NOT NULL,
    `EndTime` timestamp NOT NULL,
    `Price` decimal(10, 2) NOT NULL,
    `SpaceType` varchar(50) NOT NULL,
    `ReservationStatus` varchar(50) NOT NULL,
    `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `CanceledAt` timestamp,
    PRIMARY KEY (`ReservationID`),
    FOREIGN KEY (`LotID`) REFERENCES `ParkingLots`(`LotID`),
    FOREIGN KEY (`VehicleID`) REFERENCES `UserVehicles`(`VehicleID`)
);

CREATE TABLE `Payments` (
    `PaymentID` int NOT NULL AUTO_INCREMENT,
    `userID` int NOT NULL,
    `PaymentStatus` varchar(50) NOT NULL,
    `PaymentTime` timestamp NULL DEFAULT NULL,
    `Amount` decimal(10, 2) NOT NULL DEFAULT '-1',
    `PaymentMethod` varchar(50) NULL,
    `PaymentMethodType` varchar(50) NULL,
    `PaymentType` varchar(50) NOT NULL,
    `RelatedID` int NOT NULL,

    PRIMARY KEY (`PaymentID`)
);

CREATE TABLE `ParkingRecords` (
    `ParkingRecordID` int NOT NULL AUTO_INCREMENT,
    `LotID` int NOT NULL,
    `PaymentID` int NOT NULL,
    `SpaceType` varchar(50) NOT NULL,
    `EntryTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `ExitTime` timestamp,
    `ReservationID` int DEFAULT NULL,
    `vehicleLicense` varchar(50) NOT NULL,

    PRIMARY KEY (`ParkingRecordID`),
    FOREIGN KEY (`LotID`) REFERENCES `ParkingLots`(`LotID`),
    FOREIGN KEY (`PaymentID`) REFERENCES `Payments`(`PaymentID`)
);