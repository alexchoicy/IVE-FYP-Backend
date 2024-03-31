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
        Free
    }

    public enum PaymentMethodType
    {
        CreditCard,
        Cash,
        MasterCard,
        VisaCard,
        ApplePay,
        GooglePay,
        SamsungPay,
        Free
    }

    public enum PaymentType
    {
        Reservation,
        ParkingFee,
        PlanRenewal
    }
}