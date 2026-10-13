using BookShop.Application.Profiles.Dtos;
using ErrorOr;
using MediatR;

namespace BookShop.Application.Profiles.Queries.GetProfile;

public class GetProfileQuery : IRequest<ErrorOr<ProfileDto>>
{

}
