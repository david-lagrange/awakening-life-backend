using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Core.Services.Abstractions.Services;

public interface ISubscriptionService
{
    // User
    Task<SubServiceCustomerDto> GetUserCustomerAsync(Guid userId);
    Task<IEnumerable<SubServiceSubscriptionDto>> GetUserSubscriptionsAsync(Guid userId);
    Task<IEnumerable<SubServicePaymentMethodDto>> GetUserPaymentMethodsAsync(Guid userId);
    Task<IEnumerable<SubServiceInvoiceDto>> GetUserInvoicesAsync(Guid userId);
    Task<SubServiceSubscriptionDto> UpdateUserSubscriptionAutoRenewalAsync(Guid userId, string subscriptionId, SubServiceSubscriptionRenewalUpdateDto stripeSubscriptionRenewalUpdateDto);
    Task CancelSubscriptionAutoRenewalAsync(Guid userId);
    Task<SubServiceSetupIntentDto> CreateSetupIntentAsync(Guid userId);

    // Admin
    Task<IEnumerable<SubServiceProductDto>> GetProductsAndPricesAsync();
    Task<IEnumerable<SubServiceCustomerDto>> GetCustomersAsync();
    Task<SubServiceCustomerDto> GetCustomerAsync(string customerId);
    Task<IEnumerable<SubServiceSubscriptionDto>> GetCustomerSubscriptionsAsync(string customerId);
    Task<IEnumerable<SubServiceInvoiceDto>> GetCustomerInvoicesAsync(string customerId);
    Task<IEnumerable<SubServicePaymentMethodDto>> GetCustomerPaymentMethodsAsync(string customerId);
    Task<SubServiceSubscriptionDto> UpdateSubscriptionAutoRenewalAsync(string customerId, string subscriptionId, SubServiceSubscriptionRenewalUpdateDto stripeSubscriptionRenewalUpdateDto);
}
