using BookShop.Application.UserManagement.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.UserManagement.Queries.GetUsers;

public class GetUsersQuery:IRequest<ErrorOr<List<UserListItemDto>>>
{
    public string? Search { get; set; }
}
