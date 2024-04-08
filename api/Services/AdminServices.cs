using api.Enums;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Respone;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.Services;

public interface IAdminServices
{
    AdminDashBoardResponseDto GetDashboardData(int lotID);
    Task<AdminAnalyticsResponseDto> GetAnalyticsData(int lotID);
    Task<PaymentAnalytics> GetPaymentAnalytics(int lotID);
}

public class AdminServices : IAdminServices
{
    private readonly NormalDataBaseContext normalDataBaseContext;

    public AdminServices(NormalDataBaseContext normalDataBaseContext)
    {
        this.normalDataBaseContext = normalDataBaseContext;
    }

    public AdminDashBoardResponseDto GetDashboardData(int lotID)
    {
        int currentParkingInRegular = normalDataBaseContext.ParkingRecords
            .Count(p => p.exitTime == null && p.lotID == lotID && p.spaceType == SpaceType.REGULAR);
        int totalSpaceInRegular = normalDataBaseContext.ParkingLots
            .Where(p => p.lotID == lotID)
            .Select(p => p.regularSpaces).FirstOrDefault();

        int currentParkingInElectric = normalDataBaseContext.ParkingRecords
            .Count(p => p.exitTime == null && p.lotID == lotID && p.spaceType == SpaceType.ELECTRIC);
        int totalSpaceInElectric = normalDataBaseContext.ParkingLots
            .Where(p => p.lotID == lotID)
            .Select(p => p.electricSpaces).FirstOrDefault();

        string[] todayParkingLabels = new string[24];
        int[] todayParkingData = new int[24];
        int todayParkingInTotal = 0;

        DateTime roundedTimeInDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        for (int i = 0; i < 24; i++)
        {
            todayParkingLabels[i] = roundedTimeInDay.AddHours(i).ToString("HH:mm");
            todayParkingData[i] = normalDataBaseContext.ParkingRecordSessions
                .Count(p => p.lotID == lotID && p.CreatedAt.Hour == roundedTimeInDay.AddHours(i).Hour
                                             && p.CreatedAt.Date == roundedTimeInDay.Date);
            todayParkingInTotal += todayParkingData[i];
        }

        ChartjsData<int> todayParking = new ChartjsData<int>
        {
            labels = todayParkingLabels,
            data = todayParkingData
        };

        string[] next12HourBookedNumberLabels = new string[12];
        int[] next12HourBookedNumberData = new int[12];
        DateTime roundedTimeInHour =
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

        for (int i = 0; i < 12; i++)
        {
            next12HourBookedNumberLabels[i] = roundedTimeInHour.AddHours(i).ToString("HH:mm");
            next12HourBookedNumberData[i] = normalDataBaseContext.HourlyReservationCounts
                .Where(p => p.lotID == lotID && p.dateTime.Hour == roundedTimeInHour.AddHours(i).Hour
                                             && p.dateTime.Date == roundedTimeInHour.Date)
                .Sum(p => p.regularSpaceCount + p.electricSpaceCount);
        }

        ChartjsData<int> next12HourBookedNumber = new ChartjsData<int>
        {
            labels = next12HourBookedNumberLabels,
            data = next12HourBookedNumberData
        };

        AdminDashBoardResponseDto adminDashBoardResponseDto = new AdminDashBoardResponseDto
        {
            currentParkingInElectric = currentParkingInElectric,
            currentParkingInRegular = currentParkingInRegular,
            totalSpaceInElectric = totalSpaceInElectric,
            totalSpaceInRegular = totalSpaceInRegular,
            todayParkingInTotal = todayParkingInTotal,
            todayParking = todayParking,
            next12HourBookedNumber = next12HourBookedNumber
        };
        return adminDashBoardResponseDto;
    }

    public ChartjsData<int> GetNumberOfCarByMonthOld(int lotID, DateTime startDate)
    {
        DateTime roundedTimeInMonth = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0);
        int numberOfCarInMonthRange = DateTime.DaysInMonth(startDate.Year, startDate.Month);
        int[] numberOfCarInMonthData = new int[numberOfCarInMonthRange];

        for (int i = 0; i < numberOfCarInMonthRange; i++)
        {
            numberOfCarInMonthData[i] = normalDataBaseContext.ParkingRecords
                .Count(p => p.lotID == lotID && p.entryTime.Date == roundedTimeInMonth.AddDays(i));
        }

        ChartjsData<int> numberOfCarInMonth = new ChartjsData<int>()
        {
            labels = Enumerable.Range(0, numberOfCarInMonthRange).Select(p => p.ToString()).ToArray(),
            data = numberOfCarInMonthData
        };

        return numberOfCarInMonth;
    }


    public async Task<AdminAnalyticsResponseDto> GetAnalyticsData(int lotID)
    {
        PaymentAnalytics paymentAnalytics = await GetPaymentAnalytics(lotID);
        ParkingAnalytics parkingAnalytics = await GetParkingAnalytics(lotID);
        return new AdminAnalyticsResponseDto
        {
            parkingAnalytics = parkingAnalytics,
            paymentAnalytics = paymentAnalytics
        };
    }

    public async Task<PaymentAnalytics> GetPaymentAnalytics(int lotID)
    {
        PaymentMethod[] paymentMethods = (PaymentMethod[])Enum.GetValues(typeof(PaymentMethod));
        int[] paymentMethodData = new int[paymentMethods.Length];

        for (int i = 0; i < paymentMethods.Length; i++)
        {
            paymentMethodData[i] = await normalDataBaseContext.Payments
                .CountAsync(p => p.paymentMethod == paymentMethods[i]);
        }

        ChartjsData<int> paymentMethod = new ChartjsData<int>()
        {
            labels = paymentMethods.Select(p => p.ToString()).ToArray(),
            data = paymentMethodData
        };

        PaymentMethodType[] paymentMethodTypes = (PaymentMethodType[])Enum.GetValues(typeof(PaymentMethodType));
        int[] paymentMethodTypeData = new int[paymentMethodTypes.Length];
        for (int i = 0; i < paymentMethodTypes.Length; i++)
        {
            paymentMethodTypeData[i] = await normalDataBaseContext.Payments
                .CountAsync(p => p.paymentMethodType == paymentMethodTypes[i]);
        }

        ChartjsData<int> paymentMethodType = new ChartjsData<int>()
        {
            labels = paymentMethodTypes.Select(p => p.ToString()).ToArray(),
            data = paymentMethodTypeData
        };

        string[] priceRangeInLast7daysLabels = new string[7];
        decimal[] priceRangeInLast7daysData = new decimal[7];
        DateTime startTimeLast7Days = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
            .AddDays(-7);
        DateTime endTimeLast7Days = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        for (int i = 0; i < 7; i++)
        {
            priceRangeInLast7daysLabels[i] = startTimeLast7Days.AddDays(i).ToString("dd/MM/yyyy");
            priceRangeInLast7daysData[i] = normalDataBaseContext.Payments
                .Where(p => p.paymentType == PaymentType.ParkingFee &&
                            (p.paymentStatus == PaymentStatus.Completed
                             || p.paymentStatus == PaymentStatus.Pending) &&
                            p.createdAt.Date == startTimeLast7Days.AddDays(i))
                .Sum(p => p.amount);
        }

        ChartjsData<decimal> priceRangeInLast7days = new ChartjsData<decimal>
        {
            labels = priceRangeInLast7daysLabels,
            data = priceRangeInLast7daysData
        };


        string[] averageParkingPriceLabels = new string[7];
        decimal[] averageParkingPriceData = new decimal[7];

        for (int i = 0; i < 7; i++)
        {
            averageParkingPriceLabels[i] = startTimeLast7Days.AddDays(i).ToString("dd/MM/yyyy");
            var payments = normalDataBaseContext.Payments
                .Where(p => p.paymentType == PaymentType.ParkingFee &&
                            (p.paymentStatus == PaymentStatus.Completed
                             || p.paymentStatus == PaymentStatus.Pending) &&
                            p.createdAt.Date == startTimeLast7Days.AddDays(i));
            averageParkingPriceData[i] = payments.Any() ? payments.Average(p => p.amount) : 0;
        }

        ChartjsData<decimal> averageParkingPrice = new ChartjsData<decimal>
        {
            labels = averageParkingPriceLabels,
            data = averageParkingPriceData
        };

        return new PaymentAnalytics
        {
            paymentMethod = paymentMethod,
            paymentMethodType = paymentMethodType,
            priceRangeInLast7days = priceRangeInLast7days,
            averageParkingPriceInLast7days = averageParkingPrice
        };
    }

    public async Task<ParkingAnalytics> GetParkingAnalytics(int lotID)
    {
        DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
        SpaceType[] spaceTypes = (SpaceType[])Enum.GetValues(typeof(SpaceType));
        int[] areaAccessCountData = new int[spaceTypes.Length];
        for (int i = 0; i < spaceTypes.Length; i++)
        {
            areaAccessCountData[i] = await normalDataBaseContext.ParkingRecords
                .Where(p => p.lotID == lotID)
                .CountAsync(p => p.spaceType == spaceTypes[i]);
        }

        ChartjsData<int> areaAccessCount = new ChartjsData<int>
        {
            labels = spaceTypes.Select(p => p.ToString()).ToArray(),
            data = areaAccessCountData
        };

        int numberToGet = 24;
        string[] todayParkingLabels = new string[numberToGet];
        int[] todayParkingData = new int[numberToGet];

        DateTime roundedTimeInDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
        for (int i = numberToGet - 1; i >= 0; i--)
        {
            DateTime startHour = roundedTimeInDay.AddHours(-i);
            DateTime endHour = startHour.AddHours(1);
            todayParkingLabels[numberToGet - 1 - i] = startHour.ToString("dd/MM/yyyy HH:mm");
            todayParkingData[numberToGet - 1 - i] = normalDataBaseContext.ParkingRecordSessions
                .Count(p => p.lotID == lotID && p.CreatedAt >= startHour && p.CreatedAt < endHour);
        }

        ChartjsData<int> numberOfParkingIn24Hour = new ChartjsData<int>
        {
            labels = todayParkingLabels,
            data = todayParkingData
        };

        DateTime startTimeLast7Days =
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-7);
        DateTime endTimeLast7Days = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        ParkingTimeAnalysis parkingTimeInLast7days =
            await GetParkingTimeAnalysis(lotID, startTimeLast7Days, endTimeLast7Days);
        ReservationAnalytics reservationInLast7days =
            await GetReservationAnalytics(lotID, startTimeLast7Days, endTimeLast7Days);

        return new ParkingAnalytics
        {
            areaAccessCount = areaAccessCount,
            numberOfCarInLast24Hour = numberOfParkingIn24Hour,
            parkingTimeInLast7days = parkingTimeInLast7days,
            reservationInLast7days = reservationInLast7days
        };
    }

    public ChartjsData<int> GetNumberOfCarByMonth(int lotID, DateTime startDate)
    {
        int numberOfCarInMonthRange = DateTime.DaysInMonth(startDate.Year, startDate.Month);
        int[] numberOfCarInMonthData = new int[numberOfCarInMonthRange];

        for (int i = 0; i < numberOfCarInMonthRange; i++)
        {
            numberOfCarInMonthData[i] = normalDataBaseContext.ParkingRecords
                .Count(p => p.lotID == lotID && p.entryTime.Date == startDate.AddDays(i));
        }

        ChartjsData<int> numberOfCarInMonth = new ChartjsData<int>
        {
            labels = Enumerable.Range(0, numberOfCarInMonthRange).Select(p => p.ToString()).ToArray(),
            data = numberOfCarInMonthData
        };

        return numberOfCarInMonth;
    }

    public async Task<ParkingTimeAnalysis> GetParkingTimeAnalysis(int lotID, DateTime startTime, DateTime endTime)
    {
        double avaerageParkingTime = 0;
        double maxParkingTime = 0;
        double minParkingTime = 0;
        IEnumerable<ParkingRecords> records = normalDataBaseContext.ParkingRecords
            .Where(p => p.lotID == lotID && p.exitTime != null && p.entryTime >= startTime && p.exitTime <= endTime)
            .AsEnumerable();

        if (records.Any())
        {
            avaerageParkingTime = records.Average(p => (p.exitTime - p.entryTime).Value.TotalMinutes);
            maxParkingTime = records.Max(p => (p.exitTime - p.entryTime).Value.TotalMinutes);
            minParkingTime = records.Min(p => (p.exitTime - p.entryTime).Value.TotalMinutes);
        };

        return new ParkingTimeAnalysis
        {
            averageParkingTime = avaerageParkingTime,
            maxParkingTime = maxParkingTime,
            minParkingTime = minParkingTime
        };
    }

    public async Task<ReservationTimeAnalysis> GetReservationTimeAnalysis(int lotID, DateTime startTime,
        DateTime endTime)
    {
        double averageReservationTime = 0;
        double maxReservationTime = 0;
        double minReservationTime = 0;
        IEnumerable<Reservations> records = normalDataBaseContext.Reservations
                    .Where(p => p.lotID == lotID && p.startTime >= startTime && p.endTime <= endTime)
                    .AsEnumerable();

        if (records.Any())
        {
            averageReservationTime = records.Average(p => (p.endTime - p.startTime).TotalMinutes);
            maxReservationTime = records.Max(p => (p.endTime - p.startTime).TotalMinutes);
            minReservationTime = records.Min(p => (p.endTime - p.startTime).TotalMinutes);
        }

        return new ReservationTimeAnalysis
        {
            averageReservationTime = averageReservationTime,
            maxReservationTime = maxReservationTime,
            minReservationTime = minReservationTime
        };
    }

    public async Task<ReservationAnalytics> GetReservationAnalytics(int lotID, DateTime startTime, DateTime endTime)
    {
        int[] reservationCountData = new int[(endTime.Date - startTime.Date).Days];
        string[] reservationCountLabels = new string[(endTime.Date - startTime.Date).Days];
        for (int i = 0; i < reservationCountData.Length; i++)
        {
            reservationCountLabels[i] = startTime.Date.AddDays(i).ToString("dd/MM/yyyy");
            reservationCountData[i] = await normalDataBaseContext.HourlyReservationCounts
                .Where(p => p.lotID == lotID && p.dateTime.Date == startTime.Date.AddDays(i))
                .SumAsync(p => p.regularSpaceCount + p.electricSpaceCount);
        }

        ChartjsData<int> reservationCount = new ChartjsData<int>
        {
            labels = reservationCountLabels,
            data = reservationCountData
        };

        ReservationTimeAnalysis reservationTimeAnalysis = await GetReservationTimeAnalysis(lotID, startTime, endTime);

        SpaceType[] spaceTypes = (SpaceType[])Enum.GetValues(typeof(SpaceType));
        int[] reservationTypeData = new int[spaceTypes.Length];

        for (int i = 0; i < spaceTypes.Length; i++)
        {
            reservationTypeData[i] = await normalDataBaseContext.Reservations
                .Where(p => p.lotID == lotID && p.startTime >= startTime && p.endTime <= endTime &&
                            p.spaceType == spaceTypes[i])
                .CountAsync();
        }

        ChartjsData<int> reservationType = new ChartjsData<int>
        {
            labels = spaceTypes.Select(p => p.ToString()).ToArray(),
            data = reservationTypeData
        };

        return new ReservationAnalytics
        {
            reservationCount = reservationCount,
            reservationAboutTime = reservationTimeAnalysis,
            reservationType = reservationType
        };
    }
}