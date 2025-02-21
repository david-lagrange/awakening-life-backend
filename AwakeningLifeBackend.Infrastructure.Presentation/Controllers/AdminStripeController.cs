using AwakeningLifeBackend.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers;

[Route("api/admin/stripe")]
[Authorize(Roles = "Administrator")]
[ApiController]
public class AdminStripeController : ControllerBase
{
    private readonly IServiceManager _service;

    public AdminStripeController(IServiceManager service) => _service = service;

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _service.SubscriptionService.GetCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("customers/{customerId}")]
    public async Task<IActionResult> GetCustomer(string customerId)
    {
        var customer = await _service.SubscriptionService.GetCustomerAsync(customerId);
        return Ok(customer);
    }

    [HttpGet("customers/{customerId}/subscriptions")]
    public async Task<IActionResult> GetCustomerSubscriptions(string customerId)
    {
        var subscriptions = await _service.SubscriptionService.GetCustomerSubscriptionsAsync(customerId);
        return Ok(subscriptions);
    }

    [HttpGet("customers/{customerId}/payment-methods")]
    public async Task<IActionResult> GetCustomerPaymentMethods(string customerId)
    {
        var paymentMethods = await _service.SubscriptionService.GetCustomerPaymentMethodsAsync(customerId);
        return Ok(paymentMethods);
    }

    [HttpGet("customers/{customerId}/invoices")]
    public async Task<IActionResult> GetCustomerInvoices(string customerId)
    {
        var invoices = await _service.SubscriptionService.GetCustomerInvoicesAsync(customerId);
        return Ok(invoices);
    }

    [HttpPatch("customers/{customerId}/subscriptions/{subscriptionId}/auto-renewal")]
    public async Task<IActionResult> UpdateSubscriptionAutoRenewal(
        string customerId, 
        string subscriptionId,
        [FromBody] SubServiceSubscriptionRenewalUpdateDto stripeSubscriptionRenewalUpdateDto)
    {
        var subscription = await _service.SubscriptionService.UpdateSubscriptionAutoRenewalAsync(
            customerId, 
            subscriptionId,
            stripeSubscriptionRenewalUpdateDto);
            
        return Ok(subscription);
    }
}
