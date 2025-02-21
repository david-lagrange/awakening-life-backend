using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects;

public record SubscriptionFeatureDto
{
    public Guid SubscriptionFeatureId { get; set; }
    public string? FeatureText { get; set; }
    public int? FeatureOrder { get; set; }
    public bool IsIncluded { get; set; }
}
