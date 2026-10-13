using BookShop.Application.Orders.Users.Models;
using BookShop.Application.Сommon.Models;
using ErrorOr;

namespace BookShop.Application.Сommon.Interfaces;

public interface IPaymentService
{
    Task<ErrorOr<PaymentSessionResult>> CreateCheckoutSessionAsync(PaymentSessionRequest request);

    Task<ErrorOr<PaymentSessionStatusResult>> GetCheckoutSessionAsync(string sessionId);
}
