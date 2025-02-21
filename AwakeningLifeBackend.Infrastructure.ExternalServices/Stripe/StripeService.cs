using Stripe;

namespace AwakeningLifeBackend.Infrastructure.ExternalServices;

public class StripeService : IStripeService
{
    public StripeService()
    {
        var stripeSecret = Environment.GetEnvironmentVariable("AWAKENING_LIFE_STRIPE_SECRET");
        if (string.IsNullOrEmpty(stripeSecret))
        {
            throw new Exception("Stripe secret is not configured in the environment variables.");
        }
        StripeConfiguration.ApiKey = stripeSecret;
    }
    public async Task<Customer> CreateCustomerAsync(string email)
    {
        var options = new CustomerCreateOptions
        {
            Email = email
        };

        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(options);
        return customer;
    }

    private async Task CancelOlderSubscriptionsAsync(IEnumerable<Subscription> subscriptions)
    {
        var orderedSubscriptions = subscriptions
            .OrderByDescending(s => s.CurrentPeriodStart)
            .ToList();

        var mostRecentActiveSubscription = orderedSubscriptions
            .FirstOrDefault(s => s.Status == "active");

        if (mostRecentActiveSubscription != null)
        {
            var olderSubscriptions = orderedSubscriptions
                .Where(s => s.Status == "active" && 
                           s.CurrentPeriodStart < mostRecentActiveSubscription.CurrentPeriodStart);

            foreach (var subscription in olderSubscriptions)
            {
                await CancelSubscriptionImmediatelyAsync(subscription.Id);
            }
        }
    }

    public async Task<IEnumerable<Subscription>> GetCustomerSubscriptionsAsync(string customerId)
    {
        var listOptions = new SubscriptionListOptions
        {
            Customer = customerId,
            Expand = new List<string>
            {
                "data.items.data.price"
            }
        };

        var subscriptionService = new SubscriptionService();
        var subscriptions = await subscriptionService.ListAsync(listOptions);
        
        // Cancel older active subscriptions
        await CancelOlderSubscriptionsAsync(subscriptions.Data);
        
        // Fetch updated subscriptions after cancellations
        var updatedSubscriptions = await subscriptionService.ListAsync(listOptions);
        return updatedSubscriptions.Data;
    }

    public async Task<(IEnumerable<Product> Products, IEnumerable<Price> Prices)> GetProductsAndPricesAsync()
    {
        var productService = new ProductService();
        var productOptions = new ProductListOptions
        {
            Active = true
        };
        var products = await productService.ListAsync(productOptions);

        var priceService = new PriceService();
        var priceOptions = new PriceListOptions
        {
            Active = true
        };
        var prices = await priceService.ListAsync(priceOptions);

        return (products.Data, prices.Data);
    }

    public async Task<IEnumerable<Invoice>> GetCustomerInvoicesAsync(string customerId)
    {
        var invoiceOptions = new InvoiceListOptions
        {
            Customer = customerId,
            Expand = new List<string> { "data.charge" }
        };

        var invoiceService = new InvoiceService();
        var invoices = await invoiceService.ListAsync(invoiceOptions);
        return invoices.Data;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        var customerService = new CustomerService();
        var options = new CustomerListOptions
        {
            Limit = 100,
            Expand = new List<string>
            {
                "data.subscriptions"
            }
        };
        
        var customers = await customerService.ListAsync(options);
        
        // Cancel older active subscriptions for each customer
        foreach (var customer in customers.Data)
        {
            if (customer.Subscriptions != null)
            {
                await CancelOlderSubscriptionsAsync(customer.Subscriptions.Data);
            }
        }
        
        // Fetch updated customer data after cancellations
        customers = await customerService.ListAsync(options);
        return customers.Data;
    }

    public async Task<(IEnumerable<Customer> Customers, IEnumerable<Invoice> Invoices)> GetCustomersWithInvoicesAsync()
    {
        var customers = await GetAllCustomersAsync();
        var invoices = new List<Invoice>();

        foreach (var customer in customers)
        {
            var customerInvoices = await GetCustomerInvoicesAsync(customer.Id);
            invoices.AddRange(customerInvoices);
        }

        return (customers, invoices);
    }

    public async Task<Customer> GetCustomerByIdAsync(string customerId)
    {
        var customerService = new CustomerService();
        var options = new CustomerGetOptions
        {
            Expand = new List<string>
            {
                "subscriptions"
            }
        };
        var customer = await customerService.GetAsync(customerId, options);
        
        // Cancel older active subscriptions
        if (customer.Subscriptions != null)
        {
            await CancelOlderSubscriptionsAsync(customer.Subscriptions.Data);
            // Refresh customer data after cancellations
            customer = await customerService.GetAsync(customerId, options);
        }
        
        return customer;
    }

    public async Task<IEnumerable<PaymentMethod>> GetCustomerPaymentMethodsAsync(string customerId)
    {
        var options = new PaymentMethodListOptions
        {
            Customer = customerId,
            Type = "card"
        };

        var service = new PaymentMethodService();
        var paymentMethods = await service.ListAsync(options);
        return paymentMethods.Data;
    }

    public async Task<bool> IsUserPaidSubscriber(string? stripeCustomerId)
    {
        if (string.IsNullOrEmpty(stripeCustomerId)) return false;

        var customerService = new CustomerService();
        try 
        {
            var customer = await customerService.GetAsync(stripeCustomerId, 
                new CustomerGetOptions { Expand = new List<string> { "subscriptions" } });
            
            return customer.Subscriptions?.Data
                .Any(s => s.Status == "active") ?? false;
        }
        catch (StripeException)
        {
            return false;
        }
    }

    public async Task<Subscription> UpdateSubscriptionAutoRenewal(string subscriptionId, bool autoRenew)
    {
        var subscriptionService = new SubscriptionService();
        var options = new SubscriptionUpdateOptions
        {
            CancelAtPeriodEnd = !autoRenew // If autoRenew is false, we want to cancel at period end
        };
        
        return await subscriptionService.UpdateAsync(subscriptionId, options);
    }

    public async Task<Subscription> CancelSubscriptionImmediatelyAsync(string subscriptionId)
    {
        var subscriptionService = new SubscriptionService();
        return await subscriptionService.CancelAsync(subscriptionId, new SubscriptionCancelOptions());
    }

    public async Task<Subscription> AddFreeSubscriptionAsync(string customerId, string priceId)
    {
        var subscriptionService = new SubscriptionService();
        var subscriptionOptions = new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = priceId,
                },
            },
            // Add trial period that never ends for free subscription
            TrialEnd = DateTime.MaxValue.ToUniversalTime(),
        };

        var createdSub = await subscriptionService.CreateAsync(subscriptionOptions);

        return createdSub;
    }
}