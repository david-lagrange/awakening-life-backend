using AwakeningLifeBackend.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace AwakeningLifeBackend.Infrastructure.Presentation.Controllers;

[Route("api/stripe")]
[Authorize]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly IServiceManager _service;

    public SubscriptionController(IServiceManager service) => _service = service;

    [HttpGet("products")]
    public async Task<IActionResult> GetProductsAndPrices()
    {
        var products = await _service.SubscriptionService.GetProductsAndPricesAsync();

        return Ok(products);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomer()
    {
        var userId = User.FindFirst("userId")?.Value;

        var customer = await _service.SubscriptionService.GetUserCustomerAsync(Guid.Parse(userId ?? ""));
        return Ok(customer);
    }

    [HttpGet("customers/subscriptions")]
    public async Task<IActionResult> GetCustomerSubscriptions()
    {
        var userId = User.FindFirst("userId")?.Value;

        var subscriptions = await _service.SubscriptionService.GetUserSubscriptionsAsync(Guid.Parse(userId ?? ""));
        return Ok(subscriptions);
    }

    [HttpGet("customers/payment-methods")]
    public async Task<IActionResult> GetCustomerPaymentMethods()
    {
        var userId = User.FindFirst("userId")?.Value;

        var paymentMethods = await _service.SubscriptionService.GetUserPaymentMethodsAsync(Guid.Parse(userId ?? ""));
        return Ok(paymentMethods);
    }

    [HttpGet("customers/invoices")]
    public async Task<IActionResult> GetCustomerInvoices()
    {
        var userId = User.FindFirst("userId")?.Value;
        
        var invoices = await _service.SubscriptionService.GetUserInvoicesAsync(Guid.Parse(userId ?? ""));
        return Ok(invoices);
    }

    [HttpPatch("customers/subscriptions/{subscriptionId}/auto-renewal")]
    public async Task<IActionResult> UpdateSubscriptionAutoRenewal(
        string subscriptionId,
        [FromBody] SubServiceSubscriptionRenewalUpdateDto stripeSubscriptionRenewalUpdateDto)
    {
        var userId = User.FindFirst("userId")?.Value;

        var subscription = await _service.SubscriptionService.UpdateUserSubscriptionAutoRenewalAsync(
            Guid.Parse(userId ?? ""),
            subscriptionId,
            stripeSubscriptionRenewalUpdateDto);

        return Ok(subscription);
    }

    [HttpPut("customers/subscriptions/cancel")]
    public async Task<IActionResult> CancelSubscription()
    {
        var userId = User.FindFirst("userId")?.Value;

        await _service.SubscriptionService.CancelSubscriptionAutoRenewalAsync(Guid.Parse(userId ?? ""));

        return Ok();
    }

    [HttpPost("setup-intent")]
    public async Task<IActionResult> CreateSetupIntent()
    {
        var userId = User.FindFirst("userId")?.Value;
        var setupIntent = await _service.SubscriptionService.CreateSetupIntentAsync(Guid.Parse(userId ?? ""));
        
        return Ok(setupIntent);
    }

    [HttpPut("customers/payment-methods/default")]
    public async Task<IActionResult> UpdateDefaultPaymentMethod(
        [FromBody] SubServiceDefaultPaymentMethodUpdateDto defaultPaymentMethodUpdateDto)
    {
        var userId = User.FindFirst("userId")?.Value;
        await _service.SubscriptionService.UpdateDefaultPaymentMethodAsync(
            Guid.Parse(userId ?? ""),
            defaultPaymentMethodUpdateDto.PaymentMethodId);
        
        return Ok();
    }
}
