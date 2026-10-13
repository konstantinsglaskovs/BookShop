using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Users.Commands.ConfirmPayment;

public class ConfirmPaymentCommand:IRequest<ErrorOr<Guid>>
{
    public string SessionId { get; set; } = null!;
}
