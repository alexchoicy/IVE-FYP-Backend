namespace api.Enums
{
    public enum PaymentStatus
    {
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