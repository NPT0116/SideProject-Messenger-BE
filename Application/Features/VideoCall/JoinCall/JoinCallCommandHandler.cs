using Domain.Repositories;
using MediatR;

namespace Application.Features.VideoCall.JoinCall;

public class JoinCallCommandHandler : IRequestHandler<JoinCallCommand>
{
    private readonly IUserRepository _userRepository;
    public JoinCallCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    async Task IRequestHandler<JoinCallCommand>.Handle(JoinCallCommand request, CancellationToken cancellationToken)
    {
        
    }
}
