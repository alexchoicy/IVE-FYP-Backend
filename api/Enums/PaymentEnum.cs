namespace api.Enums
{
    //hi why this is small letter ZZZZZ
    public enum PaymentStatus
    {
        Generated,
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        App,
        PaymentMachine,
    }

    public enum PaymentMethodType
    {
        CreditCard,
        Cash,
        DebitCard,
        ApplePay,
        GooglePay,
        SamsungPay
    }

    public enum PaymentType
    {
        Reservation,
        ParkingFee,
        PlanRenewal
    }
}