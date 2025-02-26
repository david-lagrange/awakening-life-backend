namespace Shared.DataTransferObjects;

public record SubscriptionFeatureDto
{
    public Guid SubscriptionFeatureId { get; set; }
    public string? FeatureText { get; set; }
    public int? FeatureOrder { get; set; }
    public bool IsIncluded { get; set; }
}
