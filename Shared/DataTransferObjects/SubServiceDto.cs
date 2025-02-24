using System.Text.Json.Serialization;

namespace Shared.DataTransferObjects;

public record SubServiceProductDto
{
    public string? ProductId { get; init; }
    public bool Active { get; init; }
    public string? DefaultPriceId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime Created { get; init; }
    public DateTime Updated { get; init; }
    public IEnumerable<SubscriptionFeatureDto>? Features { get; set; }
    public IEnumerable<SubServicePriceDto>? Prices { get; set; }

}

public record SubServicePriceDto
{
    public string? PriceId { get; init; }
    [JsonIgnore]
    public string? ProductId { get; init; }
    public DateTime Created { get; init; }
    public bool Active { get; init; }
    public string? Currency { get; init; }
    public int? UnitAmount { get; init; }
    public string? RecurringInterval { get; init; }
    public int? RecurringIntervalCount { get; init; }
    public DateTime EstimatedRenewalDate { get; set; }

}

public record SubServiceCustomerDto
{
    public string? CustomerId { get; init; }
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? Phone { get; init; }
    public DateTime Created { get; init; }
    
    // Subscription Status
    public bool HasActiveSubscription { get; init; }
    public DateTime? CurrentSubscriptionEnd { get; init; }
    public bool? AutoRenewEnabled { get; init; }
    public string? CurrentProductName { get; init; }
    public string? RecurringInterval { get; init; }
    public int? RecurringIntervalCount { get; init; }
    
    // Financial Information
    public long TotalSpent { get; init; }
    public int SuccessfulPayments { get; init; }
    public DateTime? LastPaymentDate { get; init; }
}

public record SubServiceSubscriptionDto
{
    public string? SubscriptionId { get; init; }
    public string? Status { get; init; }
    public DateTime CurrentPeriodStart { get; init; }
    public DateTime CurrentPeriodEnd { get; init; }
    public bool AutoRenew { get; init; }
    public bool IsCanceled { get; init; }
    public SubServiceSubscriptionProductDto? Product { get; init; }
}

public record SubServiceSubscriptionProductDto
{
    public string? ProductId { get; init; }
    public string? PriceId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public long? LastPaidAmount { get; init; }
    public long? CurrentDefaultPrice { get; init; }
    public string? Currency { get; init; }
    public string? RecurringInterval { get; init; }
    public int? RecurringIntervalCount { get; init; }
    public IEnumerable<SubscriptionRoleDto>? Roles { get; init; }
}

public record SubscriptionRoleDto
{
    public string? RoleId { get; init; }
    public string? RoleName { get; init; }
}

public class SubServiceInvoiceDto
{
    public string? InvoiceId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? PaidAt { get; set; }
    public long AmountDue { get; set; }
    public string? Status { get; set; }
    public string? InvoicePdfUrl { get; set; }
    public string? PaymentMethodBrand { get; set; }
    public string? PaymentMethodLast4 { get; set; }
    public string? ProductName { get; set; }
}

public class SubServicePaymentMethodDto
{
    public string? PaymentMethodId { get; set; }
    public string? Brand { get; set; }
    public string? Last4 { get; set; }
    public long? ExpMonth { get; set; }
    public long? ExpYear { get; set; }
    public bool? IsDefault { get; set; }
    public string? CardholderName { get; set; }
}

public record SubServiceSubscriptionRenewalUpdateDto
{
    public bool IsAutoRenew { get; init; }
}

public record SubServiceSetupIntentDto
{
    public string? ClientSecret { get; init; }
}

public record SubServiceDefaultPaymentMethodUpdateDto
{
    public string PaymentMethodId { get; init; } = string.Empty;
}

public record SubServiceSubscriptionChangeDto
{
    public string NewPriceId { get; init; } = string.Empty;
    public string? PaymentMethodId { get; init; } = null;
    public string? CurrentSubscriptionId { get; init; } = null;
    public bool IsDowngrade { get; init; }
}