using Stripe;

namespace AwakeningLifeBackend.Infrastructure.ExternalServices;

public interface IStripeService
{
    Task<bool> IsUserPaidSubscriber(string email);
    Task<Customer> CreateCustomerAsync(string email);
    Task<(IEnumerable<Product> Products, IEnumerable<Price> Prices)> GetProductsAndPricesAsync();
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<Customer> GetCustomerByIdAsync(string customerId);
    Task<IEnumerable<Subscription>> GetCustomerSubscriptionsAsync(string customerId);
    Task<IEnumerable<Invoice>> GetCustomerInvoicesAsync(string customerId);
    Task<IEnumerable<Invoice>> GetCanceledSubscriptionInvoicesAsync(string customerId, int timeoutSeconds = 10);
    Task<IEnumerable<PaymentMethod>> GetCustomerPaymentMethodsAsync(string customerId);
    Task<(IEnumerable<Customer> Customers, IEnumerable<Invoice> Invoices)> GetCustomersWithInvoicesAsync();
    Task<Subscription> UpdateSubscriptionAutoRenewal(string subscriptionId, bool autoRenew);
    Task<Subscription> AddFreeSubscriptionAsync(string customerId, string priceId);
    Task<string> CreateSetupIntentAsync(string customerId);
    Task UpdateDefaultPaymentMethodAsync(string customerId, string paymentMethodId);
    Task DeletePaymentMethodAsync(string paymentMethodId);
    Task<IEnumerable<string>> GetSubscriptionProductIds(string customerId);
    Task<Subscription> CreateSubscriptionAsync(string customerId, string priceId, string? paymentMethodId, bool isDowngrade, DateTime? trialEnd = null);
    Task CancelSubscriptionImmediatelyAsync(string subscriptionId);
    Task<Subscription> UpdateSubscriptionPriceAsync(
        string? subscriptionId,
        string newPriceId,
        string customerId,
        string? paymentMethodId);
}
