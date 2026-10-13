using BookShop.Application.Orders.Users.Commands.ExpirePendingOrder;
using BookShop.Domain.Enums;
using BookShop.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookShop.Infrastructure.BackgroundServices;

public class PendingOrderCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PendingOrderCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var expirationTime = DateTime.UtcNow.AddMinutes(-360);

            var orderIds = await db.Orders.AsNoTracking()
                .Where(order =>
                    order.OrderStatus == OrderStatus.Pending &&
                    order.PaymentStatus == PaymentStatus.Pending &&
                    order.OrderDate <= expirationTime)
                .Select(order => order.Id)
                .ToListAsync(stoppingToken);

            foreach (var orderId in orderIds)
            {
                await mediator.Send(new ExpirePendingOrderCommand
                {
                    OrderId = orderId
                },
                    stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
        }
    }
}
