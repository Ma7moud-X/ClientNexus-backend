using Database.Models;

namespace Database.TypeExtensions;

public static class PaymentTypeExtensions
{
    public static char ToChar(this PaymentType paymentType)
    {
        return (char)paymentType;
    }

    public static PaymentType ToPaymentType(this char paymentType)
    {
        if (!Enum.IsDefined(typeof(PaymentType), (int)paymentType))
        {
            throw new ArgumentException($"Invalid payment type character: {paymentType}");
        }

        return (PaymentType)paymentType;
    }
}
