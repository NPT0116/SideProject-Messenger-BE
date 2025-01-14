using MediatR;

namespace Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<GetAllUsersResponse>;

public record GetAllUsersResponse(List<UserDto> Users);

public record UserDto(int Id, string FullName);