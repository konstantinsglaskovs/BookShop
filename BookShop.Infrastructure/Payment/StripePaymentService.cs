using BookShop.Application.Orders.Users.Models;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Application.Сommon.Models;
using BookShop.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace BookShop.Infrastructure.Payment;

public class StripePaymentService:IPaymentService
{
    private readonly StripeClient _stripeClient;
    private readonly StripeSettings _settings;

    public StripePaymentService(StripeClient stripeClient,IOptions<StripeSettings> options)
    {
        _stripeClient = stripeClient;
        _settings = options.Value;
    }

    public async Task<ErrorOr<PaymentSessionResult>>CreateCheckoutSessionAsync(PaymentSessionRequest request)
    {
        if (request.Items.Count == 0)
        {
            return Error.Validation(code: "Payment.EmptyItems",description: "Payment must contain at least one item.");
        }

        var options = new SessionCreateOptions
        {
            Mode = "payment",

            SuccessUrl = _settings.SuccessUrl,
            CancelUrl = $"{_settings.CancelUrl}?orderId={request.OrderId}",

            ClientReferenceId =request.OrderId.ToString(),

            Metadata = new Dictionary<string, string>
            {
                ["order_id"] =request.OrderId.ToString()
            },

            LineItems = request.Items
                .Select(item =>new SessionLineItemOptions
                    {
                        Quantity = item.Quantity,

                        PriceData =new SessionLineItemPriceDataOptions
                            {
                                Currency = "eur",

                                UnitAmount =(long)(item.UnitPrice * 100),

                                ProductData =new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = item.Name
                                    }

                            }
                    })
                .ToList()
        };

        var service =new SessionService(_stripeClient);

        var session =await service.CreateAsync(options);

        if (string.IsNullOrWhiteSpace(session.Url))
        {
            return Error.Failure(code: "Payment.SessionCreationFailed",description:"Failed to create Stripe checkout session.");
        }

        var expectedAmount = (long)(request.Total * 100);

        if (session.AmountTotal != expectedAmount)
        {
            return Error.Failure(code: "Payment.AmountMismatch",description: "Stripe payment amount does not match the expected amount.");
        }

        return new PaymentSessionResult
        {
            SessionId = session.Id,
            CheckoutUrl = session.Url
        };
    }

    public async Task<ErrorOr<PaymentSessionStatusResult>>GetCheckoutSessionAsync(string sessionId)
    {
        var service = new SessionService(_stripeClient);

        var session = await service.GetAsync(sessionId);

        if (!Guid.TryParse(session.ClientReferenceId,out var orderId))
        {
            return Error.Failure(code: "Payment.InvalidOrderReference",description: "Stripe session does not contain a valid order reference.");
        }

        return new PaymentSessionStatusResult
        {
            OrderId = orderId,
            PaymentStatus = session.PaymentStatus
        };
    }
}
