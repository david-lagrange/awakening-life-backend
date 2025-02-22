﻿using AwakeningLifeBackend.Core.Domain.Entities;
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

    public async Task CancelSubscriptionAutoRenewalAsync(Guid userId) => await CancelAllSubscriptionsAutoRenewalAsync(userId, await GetUserStripeCustomerId(userId));
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

            var subscriptionDto = new SubServiceSubscriptionDto
            {
                SubscriptionId = subscription.Id,
                Status = subscription.Status,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                AutoRenew = subscription.CancelAtPeriodEnd == false, // If CancelAtPeriodEnd is false, auto-renewal is on
                Product = new SubServiceSubscriptionProductDto
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    LastPaidAmount = subscriptionItem.Plan.Amount, // Amount actually paid for this subscription
                    CurrentDefaultPrice = currentDefaultPrice?.UnitAmount, // Current listed price for new subscriptions
                    Currency = subscriptionItem.Plan.Currency,
                    RecurringInterval = subscriptionItem.Plan.Interval.ToString(),
                    RecurringIntervalCount = (int?)subscriptionItem.Plan.IntervalCount
                }
            };

            subscriptionDtos.Add(subscriptionDto);
        }

        // Order the subscriptions by active then current period start
        subscriptionDtos = subscriptionDtos.OrderByDescending(s => s.CurrentPeriodStart).ToList();

        return subscriptionDtos;
    }

    public async Task<IEnumerable<SubServiceInvoiceDto>> GetCustomerInvoicesAsync(string customerId)
    {
        var invoices = await _stripeService.GetCustomerInvoicesAsync(customerId);
        var invoiceDtos = new List<SubServiceInvoiceDto>();

        foreach (var invoice in invoices)
        {
            var paymentDetails = invoice.Charge?.PaymentMethodDetails?.Card;
            
            var invoiceDto = new SubServiceInvoiceDto
            {
                InvoiceId = invoice.Id,
                Created = invoice.Created,
                PaidAt = invoice.StatusTransitions?.PaidAt,
                AmountDue = invoice.AmountDue,
                Status = invoice.Status,
                InvoicePdfUrl = invoice.InvoicePdf,
                PaymentMethodBrand = paymentDetails?.Brand,
                PaymentMethodLast4 = paymentDetails?.Last4
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

    public async Task CancelAllSubscriptionsAutoRenewalAsync(Guid userId, string customerId)
    {
        var subscriptions = await _stripeService.GetCustomerSubscriptionsAsync(customerId);
        var activeSubscriptions = subscriptions.Where(s => s.Status == "active").ToList();
        var baseUrl = Environment.GetEnvironmentVariable("API_CLIENT_REDIRECT_BASE_URL");

        if (baseUrl == null)
        {
            _logger.LogWarning("API_CLIENT_REDIRECT_BASE_URL environment variable is not set");
            throw new EnvironmentVariableNotSetException("API_CLIENT_REDIRECT_BASE_URL");
        }

        if (!activeSubscriptions.Any())
        {
            _logger.LogWarning($"No active subscriptions found for user with ID: {userId}");
            return;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"User with ID: {userId} was not found while attempting to cancel subscription auto-renewal");
            throw new UserNotFoundException(userId);
        }

        //if (user.IsCanceledSubscription)
        //{
        //    _logger.LogWarning($"User with ID: {userId} has already canceled subscription auto-renewal");
        //    throw new UserAlreadyCanceledSubscriptionException(userId);
        //}

        if (user.Email == null)
        {
            throw new UserHasNoEmailSetException(new Guid(user.Id));
        }

        foreach (var subscription in activeSubscriptions)
        {
            await _stripeService.UpdateSubscriptionAutoRenewal(subscription.Id, false);
        }

        //user.IsCanceledSubscription = true;
        //await _userManager.UpdateAsync(user);

        var subscriptionsLink = $"{baseUrl}/subscriptions";

        await _emailService.SendSubscriptionCanceledEmailAsync(user.Email, subscriptionsLink);
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

}
