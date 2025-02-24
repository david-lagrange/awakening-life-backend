using Stripe;
using System.Text.Json;

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
        
        return subscriptions.Data;
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
            Expand = new List<string> { 
                "data.charge",
                "data.lines"
            }
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
        
        return customer;
    }

    public async Task<IEnumerable<PaymentMethod>> GetCustomerPaymentMethodsAsync(string customerId)
    {
        var options = new PaymentMethodListOptions
        {
            Customer = customerId,
            Type = "card",
            Expand = new List<string> { "data.billing_details" }
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
            CancelAtPeriodEnd = true  // This will turn off auto-renewal
        };

        var createdSub = await subscriptionService.CreateAsync(subscriptionOptions);

        return createdSub;
    }

    public async Task<string> CreateSetupIntentAsync(string customerId)
    {
        var service = new SetupIntentService();
        var options = new SetupIntentCreateOptions
        {
            Customer = customerId,
            PaymentMethodTypes = new List<string> { "card" },
            Usage = "off_session" // This allows the payment method to be used for future payments
        };

        var setupIntent = await service.CreateAsync(options);
        return setupIntent.ClientSecret;
    }

    public async Task UpdateDefaultPaymentMethodAsync(string customerId, string paymentMethodId)
    {
        var customerService = new CustomerService();
        var options = new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions
            {
                DefaultPaymentMethod = paymentMethodId
            }
        };
        
        await customerService.UpdateAsync(customerId, options);
    }

    public async Task DeletePaymentMethodAsync(string paymentMethodId)
    {
        var service = new PaymentMethodService();
        await service.DetachAsync(paymentMethodId);
    }

    public async Task<string> GetSubscriptionProductId(string customerId)
    {
        var subscriptions = await GetCustomerSubscriptionsAsync(customerId);
        var freePriceId = Environment.GetEnvironmentVariable("AWAKENING_LIFE_STRIPE_FREE_PRICE_ID")!;
        
        // Use the same ordering logic as SubscriptionService
        var currentSubscription = subscriptions
            .OrderByDescending(s => 
                s.Status == "active" && 
                s.Items.Data.FirstOrDefault()?.Price?.Id != freePriceId && 
                s.CurrentPeriodEnd > DateTime.UtcNow)
            .ThenByDescending(s => s.CurrentPeriodEnd)
            .ThenByDescending(s => s.CurrentPeriodStart)
            .FirstOrDefault();

        if (currentSubscription == null || 
            currentSubscription.Items.Data.FirstOrDefault()?.Price?.ProductId == null)
        {
            return string.Empty;
        }

        return currentSubscription.Items.Data.First().Price.ProductId;
    }

    public async Task<Subscription> CreateSubscriptionAsync(string customerId, string priceId, string paymentMethodId)
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
            PaymentSettings = new SubscriptionPaymentSettingsOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
            },
            DefaultPaymentMethod = paymentMethodId,
            CollectionMethod = "charge_automatically",
            Expand = new List<string> { "latest_invoice.payment_intent" }
        };

        var subscription = await subscriptionService.CreateAsync(subscriptionOptions);

        // Check if payment needs additional action
        if (subscription.LatestInvoice?.PaymentIntent?.Status == "requires_action")
        {
            throw new Exception("This payment requires additional action from the customer.");
        }

        // Check if payment failed
        if (subscription.LatestInvoice?.PaymentIntent?.Status == "requires_payment_method")
        {
            throw new Exception("Payment failed. Please try again with a different payment method.");
        }

        return subscription;
    }
}