using BookShop.Domain.Enums;

namespace BookShop.Domain.Entities;

public class OrderAddress
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string City { get; set; } = null!;
    public string? State { get; set; }
    public string PostalCode { get; set; } = null!;
    public Country Country { get; set; }

   
}
