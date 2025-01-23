using System;
using Application.Users.Queries.GetAllUsers;
using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries.GetUserFromParticipantIdQuery;

public class GetUserFromParticipantQueryHandler : IRequestHandler<GetUserFromParticipantId.GetUserFromParticipantIdQuery, Response<User>>
{
    private readonly IUserRepository _userRepository;
    public GetUserFromParticipantQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Response<User>> Handle(GetUserFromParticipantId.GetUserFromParticipantIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserFromParticipantId(request.ParticipantId);

        return new Response<User>(user);
    }
}
