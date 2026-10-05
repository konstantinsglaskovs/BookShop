namespace BookShop.Application.Auth;

public class AppRoles
{
    public const string User = nameof(User);
    public const string Admin = nameof(Admin);

    public static readonly string[] All =
    [
        User,
        Admin
    ];
}
