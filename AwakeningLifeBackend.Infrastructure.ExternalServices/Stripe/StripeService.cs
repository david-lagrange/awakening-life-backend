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

    public async Task<IEnumerable<Subscription>> GetCustomerSubscriptionsAsync(string customerId)
    {
        try
        {
            Console.WriteLine($"Starting GetCustomerSubscriptionsAsync for customer: {customerId}");
            
            var allSubscriptions = new List<Subscription>();
            var subscriptionService = new SubscriptionService();
            string? lastId = null;
            bool hasMore = true;
            int batchSize = 100;
            int batchCount = 0;
            
            // Use pagination to fetch all subscriptions in batches
            while (hasMore)
            {
                try
                {
                    batchCount++;
                    Console.WriteLine($"Fetching batch #{batchCount} of subscriptions for customer: {customerId}");
                    
                    var listOptions = new SubscriptionListOptions
                    {
                        Customer = customerId,
                        Expand = new List<string>
                        {
                            "data.items.data.price",
                            "data.latest_invoice",
                            "data.default_payment_method"
                        },
                        Status = "all", // This will include all subscriptions: active, past_due, unpaid, canceled, incomplete, incomplete_expired, trialing
                        Limit = batchSize
                    };
                    
                    // Add starting_after parameter for pagination if we have a last ID
                    if (!string.IsNullOrEmpty(lastId))
                    {
                        Console.WriteLine($"Using pagination with starting_after: {lastId}");
                        listOptions.StartingAfter = lastId;
                    }
                    
                    var subscriptionBatch = await subscriptionService.ListAsync(listOptions);
                    Console.WriteLine($"Retrieved {subscriptionBatch.Data.Count} subscriptions in batch #{batchCount}");
                    
                    allSubscriptions.AddRange(subscriptionBatch.Data);
                    
                    // Check if we need to fetch more
                    hasMore = subscriptionBatch.HasMore;
                    Console.WriteLine($"HasMore: {hasMore}");
                    
                    // Update the last ID for the next batch
                    if (hasMore && subscriptionBatch.Data.Count > 0)
                    {
                        lastId = subscriptionBatch.Data.Last().Id;
                        Console.WriteLine($"Last ID for next batch: {lastId}");
                    }
                    
                    // Safety check - if we've done too many batches, break to prevent infinite loops
                    if (batchCount >= 10)
                    {
                        Console.WriteLine("Reached maximum batch count (10), stopping pagination");
                        hasMore = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching batch #{batchCount}: {ex.Message}");
                    // Continue with the next batch if possible
                    if (string.IsNullOrEmpty(lastId))
                    {
                        // If we don't have a lastId, we can't continue pagination
                        hasMore = false;
                    }
                }
            }
            
            Console.WriteLine($"Total subscriptions retrieved: {allSubscriptions.Count}");
            return allSubscriptions;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCustomerSubscriptionsAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
            }
            
            // Return empty list on error
            return new List<Subscription>();
        }
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
        try
        {
            Console.WriteLine($"Starting GetCustomerInvoicesAsync for customer: {customerId}");
            
            // Define all possible invoice statuses
            var statuses = new List<string> 
            { 
                "draft", "open", "paid", "uncollectible", "void" 
            };
            
            var invoiceService = new InvoiceService();
            
            // Create tasks for fetching invoices for each status in parallel
            var invoiceTasks = statuses.Select(status => 
            {
                Console.WriteLine($"Creating task to fetch invoices with status: {status}");
                var invoiceOptions = new InvoiceListOptions
                {
                    Customer = customerId,
                    Status = status,
                    Expand = new List<string> { 
                        "data.charge",
                        "data.lines.data.price",
                        "data.subscription"
                    },
                    Limit = 100 // Increase limit to get more invoices per request
                };
                
                return invoiceService.ListAsync(invoiceOptions);
            }).ToList();
            
            Console.WriteLine($"Created {invoiceTasks.Count} tasks for fetching invoices by status");
            
            // Also create a task for fetching canceled subscription invoices with a timeout
            Console.WriteLine("Starting task to fetch canceled subscription invoices");
            var canceledInvoicesTask = GetCanceledSubscriptionInvoicesAsync(customerId, 15); // Increased timeout to 15 seconds
            
            // Wait for all status-based invoice tasks to complete
            Console.WriteLine("Waiting for all status-based invoice tasks to complete");
            await Task.WhenAll(invoiceTasks);
            Console.WriteLine("All status-based invoice tasks completed");
            
            // Combine all results
            var allInvoices = new List<Invoice>();
            foreach (var task in invoiceTasks)
            {
                var invoices = task.Result.Data;
                Console.WriteLine($"Retrieved {invoices.Count} invoices from status task");
                allInvoices.AddRange(invoices);
            }
            
            Console.WriteLine($"Total invoices from status tasks: {allInvoices.Count}");
            
            // Add invoices from canceled subscriptions (if available within timeout)
            Console.WriteLine("Waiting for canceled subscription invoices task to complete");
            var canceledInvoices = await canceledInvoicesTask;
            Console.WriteLine($"Retrieved {canceledInvoices.Count()} invoices from canceled subscriptions");
            
            // Combine all invoices and remove duplicates
            var result = allInvoices
                .Union(canceledInvoices, new InvoiceComparer())
                .ToList();
                
            Console.WriteLine($"Final total invoices after removing duplicates: {result.Count}");
            
            // If we got no invoices, try the fallback approach
            if (result.Count == 0)
            {
                Console.WriteLine("No invoices found with parallel approach, trying fallback method");
                return await GetCustomerInvoicesFallbackAsync(customerId);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            // Log the error (in a production environment)
            Console.WriteLine($"Error retrieving invoices: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
            }
            
            // Try the fallback approach
            Console.WriteLine("Error occurred, trying fallback method");
            return await GetCustomerInvoicesFallbackAsync(customerId);
        }
    }
    
    // Fallback method to get invoices using a simpler approach
    private async Task<IEnumerable<Invoice>> GetCustomerInvoicesFallbackAsync(string customerId)
    {
        try
        {
            Console.WriteLine($"Starting fallback method to get invoices for customer: {customerId}");
            
            // First try to get all invoices directly without specifying status
            var invoiceService = new InvoiceService();
            var invoiceOptions = new InvoiceListOptions
            {
                Customer = customerId,
                Limit = 100
            };
            
            var invoices = await invoiceService.ListAsync(invoiceOptions);
            Console.WriteLine($"Fallback method retrieved {invoices.Data.Count} invoices directly");
            
            // If we got invoices, return them
            if (invoices.Data.Count > 0)
            {
                return invoices.Data;
            }
            
            // If no invoices were found, try to get them by subscription
            Console.WriteLine("No invoices found directly, trying to get them by subscription");
            var subscriptions = await GetCustomerSubscriptionsAsync(customerId);
            Console.WriteLine($"Retrieved {subscriptions.Count()} subscriptions");
            
            var allInvoices = new List<Invoice>();
            
            // Process each subscription sequentially to avoid timeout issues
            foreach (var subscription in subscriptions)
            {
                var subscriptionInvoices = await GetInvoicesBySubscriptionIdAsync(customerId, subscription.Id);
                allInvoices.AddRange(subscriptionInvoices);
            }
            
            Console.WriteLine($"Fallback method retrieved {allInvoices.Count} invoices by subscription");
            return allInvoices;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in fallback method: {ex.Message}");
            // Return empty list as last resort
            return new List<Invoice>();
        }
    }

    // Helper class to compare invoices and remove duplicates
    private class InvoiceComparer : IEqualityComparer<Invoice>
    {
        public bool Equals(Invoice? x, Invoice? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(Invoice obj)
        {
            return obj.Id.GetHashCode();
        }
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

    public async Task<IEnumerable<string>> GetSubscriptionProductIds(string customerId)
    {
        var subscriptions = await GetCustomerSubscriptionsAsync(customerId);
        var freePriceId = Environment.GetEnvironmentVariable("AWAKENING_LIFE_STRIPE_FREE_PRICE_ID")!;
        
        // Get all active or trialing subscriptions that aren't the free tier
        var activeSubscriptions = subscriptions
            .Where(s => (s.Status == "active" || s.Status == "trialing") && 
                   s.Items.Data.FirstOrDefault()?.Price?.Id != freePriceId);
            
        // Extract all product IDs from these subscriptions
        var productIds = activeSubscriptions
            .SelectMany(s => s.Items.Data)
            .Where(item => item.Price?.ProductId != null)
            .Select(item => item.Price.ProductId)
            .Distinct()
            .ToList();
            
        return productIds;
    }

    public async Task<Subscription> CreateSubscriptionAsync(
        string customerId, 
        string priceId, 
        string? paymentMethodId, 
        bool isDowngrade,
        DateTime? trialEnd = null)
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
            CollectionMethod = "charge_automatically",
            Expand = new List<string> { "latest_invoice.payment_intent" }
        };

        // Only set payment related options for upgrades or if payment method is provided
        if (!isDowngrade || !string.IsNullOrEmpty(paymentMethodId))
        {
            subscriptionOptions.PaymentSettings = new SubscriptionPaymentSettingsOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
            };
            subscriptionOptions.DefaultPaymentMethod = paymentMethodId;
        }

        // If downgrading and trial end date is provided, set it
        if (isDowngrade && trialEnd.HasValue)
        {
            subscriptionOptions.TrialEnd = trialEnd.Value;
        }

        var subscription = await subscriptionService.CreateAsync(subscriptionOptions);

        // Only check payment status for upgrades (non-downgrades)
        if (!isDowngrade)
        {
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
        }

        return subscription;
    }

    public async Task CancelSubscriptionImmediatelyAsync(string subscriptionId)
    {
        var subscriptionService = new SubscriptionService();
        await subscriptionService.CancelAsync(subscriptionId, new SubscriptionCancelOptions
        {
            InvoiceNow = false,
            Prorate = false
        });
    }

    public async Task<Subscription> UpdateSubscriptionPriceAsync(
        string? subscriptionId,
        string newPriceId,
        string customerId,
        string? paymentMethodId)
    {
        var subscriptionService = new SubscriptionService();

        // If no existing subscription, create a new one
        if (string.IsNullOrEmpty(subscriptionId))
        {
            var createOptions = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = newPriceId,
                    }
                },
                DefaultPaymentMethod = paymentMethodId
            };

            return await subscriptionService.CreateAsync(createOptions);
        }

        // Otherwise update the existing subscription
        var updateOptions = new SubscriptionUpdateOptions
        {
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Id = await GetSubscriptionItemId(subscriptionId),
                    Price = newPriceId,
                }
            },
            ProrationBehavior = "always_invoice", // This creates an immediate invoice for the price difference
            DefaultPaymentMethod = paymentMethodId
        };

        return await subscriptionService.UpdateAsync(subscriptionId, updateOptions);
    }

    private async Task<string> GetSubscriptionItemId(string subscriptionId)
    {
        var subscriptionService = new SubscriptionService();
        var subscription = await subscriptionService.GetAsync(subscriptionId);
        return subscription.Items.Data[0].Id;
    }

    public async Task<IEnumerable<Invoice>> GetCanceledSubscriptionInvoicesAsync(string customerId, int timeoutSeconds = 10)
    {
        try
        {
            Console.WriteLine($"Starting GetCanceledSubscriptionInvoicesAsync for customer: {customerId} with timeout: {timeoutSeconds}s");
            
            // First, get all canceled subscriptions for the customer
            Console.WriteLine("Fetching all subscriptions to find canceled ones");
            var subscriptions = await GetCustomerSubscriptionsAsync(customerId);
            Console.WriteLine($"Retrieved {subscriptions.Count()} total subscriptions");
            
            var canceledSubscriptionIds = subscriptions
                .Where(s => s.Status == "canceled")
                .Select(s => s.Id)
                .ToList();
            
            Console.WriteLine($"Found {canceledSubscriptionIds.Count} canceled subscriptions");
            
            if (!canceledSubscriptionIds.Any())
            {
                Console.WriteLine("No canceled subscriptions found, returning empty list");
                return new List<Invoice>();
            }
            
            // Create a list to hold all invoices for canceled subscriptions
            var invoiceService = new InvoiceService();
            
            // Create tasks for fetching invoices for each subscription in parallel
            Console.WriteLine("Creating tasks to fetch invoices for each canceled subscription");
            var invoiceTasks = canceledSubscriptionIds.Select(subscriptionId => 
            {
                Console.WriteLine($"Creating task for subscription: {subscriptionId}");
                var invoiceOptions = new InvoiceListOptions
                {
                    Customer = customerId,
                    Subscription = subscriptionId,
                    Expand = new List<string> { 
                        "data.charge",
                        "data.lines.data.price",
                        "data.subscription"
                    },
                    Limit = 100
                };
                
                return invoiceService.ListAsync(invoiceOptions);
            }).ToList();
            
            Console.WriteLine($"Created {invoiceTasks.Count} tasks for fetching invoices by subscription");
            
            // Wait for all tasks to complete with a timeout
            Console.WriteLine($"Waiting for tasks to complete with timeout of {timeoutSeconds} seconds");
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
            var completedTask = await Task.WhenAny(Task.WhenAll(invoiceTasks), timeoutTask);
            
            // If timeout occurred, only process completed tasks
            var canceledInvoices = new List<Invoice>();
            if (completedTask == timeoutTask)
            {
                Console.WriteLine("Timeout occurred before all tasks completed");
                // Only process completed tasks
                var completedTasks = invoiceTasks.Where(t => t.IsCompleted && !t.IsFaulted).ToList();
                Console.WriteLine($"{completedTasks.Count} out of {invoiceTasks.Count} tasks completed before timeout");
                
                foreach (var task in completedTasks)
                {
                    var invoices = task.Result.Data;
                    Console.WriteLine($"Retrieved {invoices.Count} invoices from completed task");
                    canceledInvoices.AddRange(invoices);
                }
            }
            else
            {
                Console.WriteLine("All tasks completed successfully before timeout");
                // All tasks completed successfully
                foreach (var task in invoiceTasks)
                {
                    var invoices = task.Result.Data;
                    Console.WriteLine($"Retrieved {invoices.Count} invoices from task");
                    canceledInvoices.AddRange(invoices);
                }
            }
            
            Console.WriteLine($"Total invoices from canceled subscriptions: {canceledInvoices.Count}");
            return canceledInvoices;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCanceledSubscriptionInvoicesAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
            }
            
            return new List<Invoice>();
        }
    }

    // Helper method to get invoices for a specific subscription
    private async Task<IEnumerable<Invoice>> GetInvoicesBySubscriptionIdAsync(string customerId, string subscriptionId)
    {
        try
        {
            Console.WriteLine($"Getting invoices for subscription: {subscriptionId}");
            var invoiceService = new InvoiceService();
            var invoiceOptions = new InvoiceListOptions
            {
                Customer = customerId,
                Subscription = subscriptionId,
                Expand = new List<string> { 
                    "data.charge",
                    "data.lines.data.price",
                    "data.subscription"
                },
                Limit = 100
            };
            
            var invoices = await invoiceService.ListAsync(invoiceOptions);
            Console.WriteLine($"Retrieved {invoices.Data.Count} invoices for subscription: {subscriptionId}");
            return invoices.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting invoices for subscription {subscriptionId}: {ex.Message}");
            return new List<Invoice>();
        }
    }
}