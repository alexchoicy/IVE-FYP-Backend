namespace api.Models.Respone;

public class AdminDashBoardResponseDto
{   
    public required int currentParkingInRegular { get; set; }
    public required int currentParkingInElectric { get; set; }
    public required int totalSpaceInRegular { get; set; }
    public required int totalSpaceInElectric { get; set; }
    public required int todayParkingInTotal { get; set; }
    public required ChartjsData<int> todayParking { get; set; }
    public required ChartjsData<int> next12HourBookedNumber { get; set; }
}

public class ChartjsData<T>
{
    public required string[] labels { get; set; }
    public required T[] data { get; set; }
}

public class AdminAnalyticsResponseDto
{  
    public required PaymentAnalytics paymentAnalytics { get; set; }
    public required ParkingAnalytics parkingAnalytics { get; set; }
}

public class PaymentAnalytics
{   
    public required ChartjsData<int> paymentMethod { get; set; }
    public required ChartjsData<int> paymentMethodType { get; set; }
    public required ChartjsData<decimal> priceRangeInLast7days { get; set; }
    public required ChartjsData<decimal> averageParkingPriceInLast7days { get; set; }
}

public class ParkingAnalytics
{   
    //electric, regular
    public required ChartjsData<int> areaAccessCount { get; set; }
    public required ChartjsData<int> numberOfCarInLast24Hour { get; set; }
    // public required ChartjsData<int> numberOfCarInLast7days { get; set; }
    // public required ChartjsData<int> numberOfCarInLastMonth { get; set; }
    public required ParkingTimeAnalysis parkingTimeInLast7days { get; set; }
    //should i add Month? or another method to get the data?
    // public required ParkingTimeAnalysis parkingTimeInLastMonth { get; set; }
    public required ReservationAnalytics reservationInLast7days { get; set; }
    // public required ReservationAnalytics reservationInLastMonth { get; set; }
}

public class ParkingTimeAnalysis
{
    public required double averageParkingTime { get; set; }
    public required double maxParkingTime { get; set; }
    public required double minParkingTime { get; set; }
}


public class ReservationAnalytics
{
    public required ChartjsData<int> reservationCount { get; set; }
    public required ReservationTimeAnalysis reservationAboutTime { get; set; }
    public required ChartjsData<int> reservationType { get; set; }
}

public class ReservationTimeAnalysis
{
    public required double averageReservationTime { get; set; }
    public required double maxReservationTime { get; set; }
    public required double minReservationTime { get; set; }
}
