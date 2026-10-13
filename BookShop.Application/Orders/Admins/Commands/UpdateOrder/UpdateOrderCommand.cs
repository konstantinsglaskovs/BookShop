using BookShop.Domain.Enums;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Orders.Admins.Commands.UpdateOrder;

public class UpdateOrderCommand:IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
}
