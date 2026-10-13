using BookShop.Application.UserManagement.Dtos;
using BookShop.Application.Сommon.Interfaces;
using ErrorOr;
using MediatR;

namespace BookShop.Application.UserManagement.Queries.GetUsers
{
    public class GetUsersQueryHandler:IRequestHandler<GetUsersQuery, ErrorOr<List<UserListItemDto>>>
    {
        private readonly IUserManagementService _userManagementService;

        public GetUsersQueryHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<ErrorOr<List<UserListItemDto>>> Handle(GetUsersQuery request,CancellationToken cancellationToken)
        {
            return await _userManagementService.GetUsersAsync(request.Search);
        }
    }
}
