namespace BookShop.Application.Auth.Commands.Register;

public class RegisteredUserResult
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
}
