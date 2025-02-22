namespace AwakeningLifeBackend.Core.Domain.Exceptions;


public class PaymentMethodNotFoundException : NotFoundException
{
    public PaymentMethodNotFoundException(string paymentMethodId)
        : base($"Payment method with ID: {paymentMethodId} was not found.")
    {
    }
}