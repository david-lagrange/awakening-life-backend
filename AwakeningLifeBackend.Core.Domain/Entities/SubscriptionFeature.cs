namespace AwakeningLifeBackend.Core.Domain.Entities;

public class SubscriptionFeature
{
    public Guid SubscriptionFeatureId { get; set; }
    public string? ProductId { get; set; }
    public string? FeatureText { get; set; }
    public int? FeatureOrder { get; set; }
    public bool IsIncluded { get; set; }
}
