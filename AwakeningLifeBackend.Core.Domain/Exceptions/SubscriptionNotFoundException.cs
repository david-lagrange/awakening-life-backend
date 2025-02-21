namespace AwakeningLifeBackend.Core.Domain.Exceptions;

public class SubscriptionNotFoundException : NotFoundException
{
    public SubscriptionNotFoundException(string subscriptionId)
        : base($"Subscription with id: {subscriptionId} doesn't exist for the customer.")
    {
    }
}