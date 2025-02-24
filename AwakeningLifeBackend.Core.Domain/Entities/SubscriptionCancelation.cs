namespace AwakeningLifeBackend.Core.Domain.Entities;

internal class SubscriptionCancelation
{
    public Guid SubscriptionCancelationId { get; set; }
    public string? SubscriptionId { get; set; }
    public DateTime CancelationDate { get; set; }
}
