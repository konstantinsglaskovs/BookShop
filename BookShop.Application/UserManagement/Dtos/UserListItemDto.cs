namespace BookShop.Application.UserManagement.Dtos;

public class UserListItemDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Role { get; set; } = null!;
    public bool IsLocked { get; set; }
}
