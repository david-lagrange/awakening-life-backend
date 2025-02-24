using AwakeningLifeBackend.Core.Domain.Entities;
using AwakeningLifeBackend.Core.Domain.Exceptions;
using AwakeningLifeBackend.Core.Domain.Repositories;
using AwakeningLifeBackend.Core.Services.Abstractions.Services;
using AwakeningLifeBackend.Infrastructure.ExternalServices;
using AutoMapper;
using LoggingService;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;


namespace AwakeningLifeBackend.Core.Services.Services;

internal sealed class SubscriptionService : ISubscriptionService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IStripeService _stripeService;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public SubscriptionService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IStripeService stripeService, UserManager<User> userManager, IEmailService emailService)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _stripeService = stripeService;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<SubServiceCustomerDto> GetUserCustomerAsync(Guid userId) => 
        await GetCustomerAsync(await GetUserStripeCustomerId(userId));

    public async Task<IEnumerable<SubServiceSubscriptionDto>> GetUserSubscriptionsAsync(Guid userId) =>
        await GetCustomerSubscriptionsAsync(await GetUserStripeCustomerId(userId));

    public async Task<IEnumerable<SubServicePaymentMethodDto>> GetUserPaymentMethodsAsync(Guid userId) =>
        await GetCustomerPaymentMethodsAsync(await GetUserStripeCustomerId(userId));

    public async Task<IEnumerable<SubServiceInvoiceDto>> GetUserInvoicesAsync(Guid userId) =>
        await GetCustomerInvoicesAsync(await GetUserStripeCustomerId(userId));

    public async Task<SubServiceSubscriptionDto> UpdateUserSubscriptionAutoRenewalAsync(Guid userId, string subscriptionId, SubServiceSubscriptionRenewalUpdateDto stripeSubscriptionRenewalUpdateDto) =>
        await UpdateSubscriptionAutoRenewalAsync(await GetUserStripeCustomerId(userId), subscriptionId, stripeSubscriptionRenewalUpdateDto);

    public async Task<IEnumerable<SubServiceProductDto>> GetProductsAndPricesAsync()
    {
        var (products, prices) = await _stripeService.GetProductsAndPricesAsync();

        var productDtos = _mapper.Map<IEnumerable<SubServiceProductDto>>(products);
        var priceDtos = _mapper.Map<IEnumerable<SubServicePriceDto>>(prices);

        var pricesByProductId = priceDtos.Where(p => p.ProductId != null)
                                 .GroupBy(p => p.ProductId!)
                                 .ToDictionary(g => g.Key, g => g.AsEnumerable());

        foreach (var productDto in productDtos)
        {
            if (productDto.ProductId != null && pricesByProductId.TryGetValue(productDto.ProductId, out var productPrices))
            {
                var subFeatures = await _repository.SubscriptionFeature.GetSubscriptionFeaturesAsync(productDto.ProductId, false);
                var subFeatureDtos = _mapper.Map<IEnumerable<SubscriptionFeatureDto>>(subFeatures);
                productDto.Features = subFeatureDtos;
                productDto.Prices = productPrices;
            }
        }

        productDtos = productDtos.OrderBy(pd => pd.Prices?.First().UnitAmount);

        return productDtos;
    }

    public async Task<IEnumerable<SubServiceCustomerDto>> GetCustomersAsync()
    {
        var (customers, invoices) = await _stripeService.GetCustomersWithInvoicesAsync();
        var (products, _) = await _stripeService.GetProductsAndPricesAsync();
        var customerDtos = new List<SubServiceCustomerDto>();

        foreach (var customer in customers)
        {
            var customerInvoices = invoices.Where(i => i.CustomerId == customer.Id);
            
            // Get active subscription info
            var activeSubscription = customer.Subscriptions?.Data
                .OrderByDescending(s => s.CurrentPeriodEnd)
                .FirstOrDefault(s => s.Status == "active");

            // Get product name from products list
            var productId = activeSubscription?.Items.Data.FirstOrDefault()?.Price?.ProductId;
            var productName = products.FirstOrDefault(p => p.Id == productId)?.Name;

            // Calculate financial metrics
            var totalSpent = customerInvoices
                .Where(i => i.Status == "paid")
                .Sum(i => i.AmountPaid);
            
            var successfulPayments = customerInvoices
                .Count(i => i.Status == "paid");
            
            var lastPayment = customerInvoices
                .Where(i => i.Status == "paid")
                .OrderByDescending(i => i.Created)
                .FirstOrDefault();

            var activeSubscriptionItem = activeSubscription?.Items.Data.FirstOrDefault();
            var customerDto = new SubServiceCustomerDto
            {
                CustomerId = customer.Id,
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.Phone,
                Created = customer.Created,
                
                // Subscription Status
                HasActiveSubscription = activeSubscription != null,
                CurrentSubscriptionEnd = activeSubscription?.CurrentPeriodEnd,
                AutoRenewEnabled = activeSubscription != null && !activeSubscription.CancelAtPeriodEnd,
                CurrentProductName = productName,
                RecurringInterval = activeSubscriptionItem?.Plan?.Interval.ToString(),
                RecurringIntervalCount = (int?)activeSubscriptionItem?.Plan?.IntervalCount,
                
                // Financial Information
                TotalSpent = totalSpent,
                SuccessfulPayments = successfulPayments,
                LastPaymentDate = lastPayment?.Created
            };

            customerDtos.Add(customerDto);
        }

        return customerDtos
            .OrderByDescending(c => c.CurrentSubscriptionEnd)
            .ThenByDescending(c => c.Created);
    }

    public async Task<SubServiceCustomerDto> GetCustomerAsync(string customerId)
    {
        var customer = await _stripeService.GetCustomerByIdAsync(customerId);
        var customerInvoices = await _stripeService.GetCustomerInvoicesAsync(customerId);
        var (products, _) = await _stripeService.GetProductsAndPricesAsync();

        // Get active subscription info
        var activeSubscription = customer.Subscriptions?.Data
            .OrderByDescending(s => s.CurrentPeriodEnd)
            .FirstOrDefault(s => s.Status == "active");

        // Get product name from products list
        var productId = activeSubscription?.Items.Data.FirstOrDefault()?.Price?.ProductId;
        var productName = products.FirstOrDefault(p => p.Id == productId)?.Name;

        // Calculate financial metrics
        var totalSpent = customerInvoices
            .Where(i => i.Status == "paid")
            .Sum(i => i.AmountPaid);

        var successfulPayments = customerInvoices
            .Count(i => i.Status == "paid");

        var lastPayment = customerInvoices
            .Where(i => i.Status == "paid")
            .OrderByDescending(i => i.Created)
            .FirstOrDefault();

        var activeSubscriptionItem = activeSubscription?.Items.Data.FirstOrDefault();
        return new SubServiceCustomerDto
        {
            CustomerId = customer.Id,
            Email = customer.Email,
            Name = customer.Name,
            Phone = customer.Phone,
            Created = customer.Created,

            // Subscription Status
            HasActiveSubscription = activeSubscription != null,
            CurrentSubscriptionEnd = activeSubscription?.CurrentPeriodEnd,
            AutoRenewEnabled = activeSubscription != null && !activeSubscription.CancelAtPeriodEnd,
            CurrentProductName = productName,
            RecurringInterval = activeSubscriptionItem?.Plan?.Interval.ToString(),
            RecurringIntervalCount = (int?)activeSubscriptionItem?.Plan?.IntervalCount,

            // Financial Information
            TotalSpent = totalSpent,
            SuccessfulPayments = successfulPayments,
            LastPaymentDate = lastPayment?.Created
        };
    }

    public async Task<IEnumerable<SubServiceSubscriptionDto>> GetCustomerSubscriptionsAsync(string customerId)
    {
        var subscriptions = await _stripeService.GetCustomerSubscriptionsAsync(customerId);
        var (products, prices) = await _stripeService.GetProductsAndPricesAsync();
        
        var subscriptionDtos = new List<SubServiceSubscriptionDto>();

        foreach (var subscription in subscriptions)
        {
            // Get the first subscription item (assuming one product per subscription)
            var subscriptionItem = subscription.Items.Data.FirstOrDefault();
            if (subscriptionItem == null) continue;

            // Find the related product
            var product = products.FirstOrDefault(p => p.Id == subscriptionItem.Plan.ProductId);
            if (product == null) continue;

            // Find the current default price for this product
            var currentDefaultPrice = prices
                .FirstOrDefault(p => p.Id == product.DefaultPriceId);

            // Get subscription roles for the product
            var subscriptionRoles = await _repository.SubscriptionRole
                .GetSubscriptionRolesForProductAsync(product.Id, false);

            // Check if subscription has been canceled
            var cancelation = await _repository.SubscriptionCancelation
                .GetSubscriptionCancelationBySubscriptionIdAsync(subscription.Id, false);

            var subscriptionDto = new SubServiceSubscriptionDto
            {
                SubscriptionId = subscription.Id,
                Status = subscription.Status,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                AutoRenew = subscription.CancelAtPeriodEnd == false,
                IsCanceled = cancelation != null,  // Set based on cancelation record
                Product = new SubServiceSubscriptionProductDto
                {
                    ProductId = product.Id,
                    PriceId = subscriptionItem.Plan.Id,
                    Name = product.Name,
                    Description = product.Description,
                    LastPaidAmount = subscriptionItem.Plan.Amount,
                    CurrentDefaultPrice = currentDefaultPrice?.UnitAmount,
                    Currency = subscriptionItem.Plan.Currency,
                    RecurringInterval = subscriptionItem.Plan.Interval.ToString(),
                    RecurringIntervalCount = (int?)subscriptionItem.Plan.IntervalCount,
                    Roles = subscriptionRoles.Select(sr => new SubscriptionRoleDto
                    {
                        RoleId = sr.RoleId,
                        RoleName = sr.Role?.Name
                    })
                }
            };

            subscriptionDtos.Add(subscriptionDto);
        }

        var freePriceId = Environment.GetEnvironmentVariable("AWAKENING_LIFE_STRIPE_FREE_PRICE_ID")!;

        if (string.IsNullOrEmpty(freePriceId))
        {
            _logger.LogError("Failed to get free price ID from environment variables.");
            throw new EnvironmentVariableNotSetException("Failed to get free price ID from environment variables.");
        }

        subscriptionDtos = subscriptionDtos
            .OrderByDescending(s => 
                s.Status == "active" &&
                s.Product?.PriceId != freePriceId && 
                s.CurrentPeriodEnd > DateTime.UtcNow)
            .ThenByDescending(s => s.CurrentPeriodEnd)
            .ThenByDescending(s => s.CurrentPeriodStart)
            .ToList();

        return subscriptionDtos;
    }

    public async Task<IEnumerable<SubServiceInvoiceDto>> GetCustomerInvoicesAsync(string customerId)
    {
        var invoices = await _stripeService.GetCustomerInvoicesAsync(customerId);
        var invoiceDtos = new List<SubServiceInvoiceDto>();
        var freePriceId = Environment.GetEnvironmentVariable("AWAKENING_LIFE_STRIPE_FREE_PRICE_ID")!;

        foreach (var invoice in invoices)
        {
            var lineItem = invoice.Lines.Data.FirstOrDefault();
            if (lineItem?.Price?.Id == freePriceId || invoice.AmountDue == 0) continue; // Skip free subscription invoices

            var paymentDetails = invoice.Charge?.PaymentMethodDetails?.Card;
            
            var description = lineItem?.Description;
            var productName = description?.Split('×')
                             .Skip(1)
                             .FirstOrDefault()
                             ?.Split('(')
                             .FirstOrDefault()
                             ?.Trim();

            var invoiceDto = new SubServiceInvoiceDto
            {
                InvoiceId = invoice.Id,
                Created = invoice.Created,
                PaidAt = invoice.StatusTransitions?.PaidAt,
                AmountDue = invoice.AmountDue,
                Status = invoice.Status,
                InvoicePdfUrl = invoice.InvoicePdf,
                PaymentMethodBrand = paymentDetails?.Brand,
                PaymentMethodLast4 = paymentDetails?.Last4,
                ProductName = productName
            };

            invoiceDtos.Add(invoiceDto);
        }

        return invoiceDtos;
    }

    public async Task<IEnumerable<SubServicePaymentMethodDto>> GetCustomerPaymentMethodsAsync(string customerId)
    {
        var paymentMethods = await _stripeService.GetCustomerPaymentMethodsAsync(customerId);
        var customer = await _stripeService.GetCustomerByIdAsync(customerId);
        
        var paymentMethodDtos = new List<SubServicePaymentMethodDto>();

        foreach (var pm in paymentMethods)
        {
            var paymentMethodDto = new SubServicePaymentMethodDto
            {
                PaymentMethodId = pm.Id,
                Brand = pm.Card.Brand,
                Last4 = pm.Card.Last4,
                ExpMonth = pm.Card.ExpMonth,
                ExpYear = pm.Card.ExpYear,
                IsDefault = pm.Id == customer.InvoiceSettings?.DefaultPaymentMethodId,
                CardholderName = pm.BillingDetails?.Name
            };

            paymentMethodDtos.Add(paymentMethodDto);
        }

        return paymentMethodDtos;
    }

    public async Task<SubServiceSubscriptionDto> UpdateSubscriptionAutoRenewalAsync(string customerId, string subscriptionId, SubServiceSubscriptionRenewalUpdateDto stripeSubscriptionRenewalUpdateDto)
    {
        var subscriptions = await _stripeService.GetCustomerSubscriptionsAsync(customerId);
        var subscription = subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        
        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(subscriptionId);
        }

        var updatedSubscription = await _stripeService.UpdateSubscriptionAutoRenewal(subscriptionId, stripeSubscriptionRenewalUpdateDto.IsAutoRenew);
        
        var allSubscriptions = await GetCustomerSubscriptionsAsync(customerId);
        return allSubscriptions.First(s => s.SubscriptionId == subscriptionId);
    }

    private async Task<string> GetUserStripeCustomerId(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"User with ID: {userId} was not found while attempting aircraft operation");
            throw new UserNotFoundException(userId);
        }

        if (user.StripeCustomerId == null)
        {
            _logger.LogWarning($"User with ID: {userId} does not have a Stripe customer ID");
            throw new UserNotFoundException("User does not have a Stripe customer ID");
        }

        return user.StripeCustomerId;
    }

    public async Task<SubServiceSetupIntentDto> CreateSetupIntentAsync(Guid userId)
    {
        var customerId = await GetUserStripeCustomerId(userId);
        var clientSecret = await _stripeService.CreateSetupIntentAsync(customerId);
        
        return new SubServiceSetupIntentDto
        {
            ClientSecret = clientSecret
        };
    }

    public async Task UpdateDefaultPaymentMethodAsync(Guid userId, string paymentMethodId)
    {
        var customerId = await GetUserStripeCustomerId(userId);
        
        // Verify the payment method belongs to the customer
        var paymentMethods = await _stripeService.GetCustomerPaymentMethodsAsync(customerId);
        var paymentMethod = paymentMethods.FirstOrDefault(pm => pm.Id == paymentMethodId);
        
        if (paymentMethod == null)
        {
            throw new PaymentMethodNotFoundException(paymentMethodId);
        }
        
        await _stripeService.UpdateDefaultPaymentMethodAsync(customerId, paymentMethodId);
    }

    public async Task DeletePaymentMethodAsync(Guid userId, string paymentMethodId)
    {
        var customerId = await GetUserStripeCustomerId(userId);
        
        // Verify the payment method belongs to the customer
        var paymentMethods = await _stripeService.GetCustomerPaymentMethodsAsync(customerId);
        var paymentMethod = paymentMethods.FirstOrDefault(pm => pm.Id == paymentMethodId);
        
        if (paymentMethod == null)
        {
            throw new PaymentMethodNotFoundException(paymentMethodId);
        }

        // Check if this is the default payment method
        var customer = await _stripeService.GetCustomerByIdAsync(customerId);
        if (paymentMethodId == customer.InvoiceSettings?.DefaultPaymentMethodId)
        {
            throw new InvalidOperationException("Cannot delete the default payment method");
        }
        
        await _stripeService.DeletePaymentMethodAsync(paymentMethodId);
    }

    public async Task<SubServiceSubscriptionDto> ChangeSubscriptionAsync(Guid userId, string newPriceId, string? paymentMethodId, string? currentSubscriptionId, bool isDowngrade)
    {
        var customerId = await GetUserStripeCustomerId(userId);
        
        // Verify the payment method belongs to the customer
        var paymentMethods = await _stripeService.GetCustomerPaymentMethodsAsync(customerId);
        var paymentMethod = paymentMethods.FirstOrDefault(pm => pm.Id == paymentMethodId);
        
        if (paymentMethod == null && !isDowngrade)
        {
            throw new PaymentMethodNotFoundException(paymentMethodId ?? "(No payment method provided)");
        }

        // If downgrading and there's an existing subscription
        if (isDowngrade && !string.IsNullOrEmpty(currentSubscriptionId))
        {
            // Get current subscription to find its end date
            var subscriptions = await _stripeService.GetCustomerSubscriptionsAsync(customerId);
            var currentSubscription = subscriptions.FirstOrDefault(s => s.Id == currentSubscriptionId);
            
            if (currentSubscription == null)
            {
                throw new SubscriptionNotFoundException(currentSubscriptionId);
            }

            // Cancel current subscription at period end
            await _stripeService.UpdateSubscriptionAutoRenewal(currentSubscriptionId, false);

            // Create new subscription that starts billing at the end of current subscription
            var subscription = await _stripeService.CreateSubscriptionAsync(
                customerId, 
                newPriceId, 
                paymentMethodId,
                isDowngrade,
                currentSubscription.CurrentPeriodEnd); // Pass the current subscription's end date
        }
        else
        {
            // Handle upgrades as before - immediate effect
            if (!string.IsNullOrEmpty(currentSubscriptionId))
            {
                await _stripeService.UpdateSubscriptionAutoRenewal(currentSubscriptionId, false);
            }

            var subscription = await _stripeService.CreateSubscriptionAsync(
                customerId, 
                newPriceId, 
                paymentMethodId,
                isDowngrade,
                null); // No trial end date for upgrades
        }
        
        // Get the updated subscription details
        var allSubscriptions = await GetCustomerSubscriptionsAsync(customerId);
        return allSubscriptions.First();
    }

    public async Task<SubServiceSubscriptionDto> CancelSubscriptionAutoRenewalAsync(Guid userId, string subscriptionId)
    {
        var customerId = await GetUserStripeCustomerId(userId);
        var subscriptions = await _stripeService.GetCustomerSubscriptionsAsync(customerId);
        var subscription = subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var baseUrl = Environment.GetEnvironmentVariable("API_CLIENT_REDIRECT_BASE_URL");
        var freePriceId = Environment.GetEnvironmentVariable("AWAKENING_LIFE_STRIPE_FREE_PRICE_ID");

        if (baseUrl == null)
        {
            _logger.LogWarning("API_CLIENT_REDIRECT_BASE_URL environment variable is not set");
            throw new EnvironmentVariableNotSetException("API_CLIENT_REDIRECT_BASE_URL");
        }

        if (freePriceId == null)
        {
            _logger.LogWarning("AWAKENING_LIFE_STRIPE_FREE_PRICE_ID environment variable is not set");
            throw new EnvironmentVariableNotSetException("AWAKENING_LIFE_STRIPE_FREE_PRICE_ID");
        }

        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        if (user.Email == null)
        {
            throw new UserHasNoEmailSetException(new Guid(user.Id));
        }

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(subscriptionId);
        }

        var updatedSubscription = await _stripeService.UpdateSubscriptionAutoRenewal(subscriptionId, false);
        
        // Check if user has an active free subscription
        var hasActiveFreeSubscription = subscriptions.Any(s => 
            s.Status == "active" && 
            s.Items.Data.Any(i => i.Price.Id == freePriceId));

        // If no active free subscription, create one
        if (!hasActiveFreeSubscription)
        {
            await _stripeService.AddFreeSubscriptionAsync(customerId, freePriceId);
        }

        // Record the cancellation
        var subscriptionCancelation = new SubscriptionCancelation
        {
            SubscriptionCancelationId = Guid.NewGuid(),
            SubscriptionId = subscriptionId,
            CancelationDate = DateTime.UtcNow,
        };

        _repository.SubscriptionCancelation.CreateSubscriptionCancelation(subscriptionCancelation);
        await _repository.SaveAsync();
        
        var subscriptionsLink = $"{baseUrl}/account/manage-subscription";

        // TODO: pass subscription active until date to email
        await _emailService.SendSubscriptionCanceledEmailAsync(user.Email, subscriptionsLink);

        var allSubscriptions = await GetCustomerSubscriptionsAsync(customerId);
        return allSubscriptions.First(s => s.SubscriptionId == subscriptionId);
    }

    public async Task<SubServiceSubscriptionDto> ReactivateSubscriptionAsync(Guid userId, string subscriptionId)
    {
        var customerId = await GetUserStripeCustomerId(userId);
        var subscriptions = await _stripeService.GetCustomerSubscriptionsAsync(customerId);
        var subscription = subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        if (user.Email == null)
        {
            throw new UserHasNoEmailSetException(new Guid(user.Id));
        }

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(subscriptionId);
        }

        // Turn auto-renewal back on
        var updatedSubscription = await _stripeService.UpdateSubscriptionAutoRenewal(subscriptionId, true);
        
        // Remove the cancellation record
        var cancellation = await _repository.SubscriptionCancelation
            .GetSubscriptionCancelationBySubscriptionIdAsync(subscriptionId, trackChanges: true);
        
        if (cancellation != null)
        {
            await _repository.SubscriptionCancelation.DeleteSubscriptionCancelationAsync(cancellation);
            await _repository.SaveAsync();
        }

        var allSubscriptions = await GetCustomerSubscriptionsAsync(customerId);
        return allSubscriptions.First(s => s.SubscriptionId == subscriptionId);
    }

}
