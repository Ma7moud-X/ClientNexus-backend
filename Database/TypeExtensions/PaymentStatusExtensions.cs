using Database.Models;

namespace Database.TypeExtensions;

public static class PaymentStatusExtensions
{
    public static char ToChar(this PaymentStatus paymentStatus)
    {
        return (char)paymentStatus;
    }

    public static PaymentStatus ToPaymentStatus(this char paymentStatus)
    {
        if (!Enum.IsDefined(typeof(PaymentStatus), (int)paymentStatus))
        {
            throw new ArgumentException($"Invalid payment status character: {paymentStatus}");
        }

        return (PaymentStatus)paymentStatus;
    }
}
