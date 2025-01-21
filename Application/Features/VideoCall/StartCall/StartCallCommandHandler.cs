using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using MediatR;

namespace Application.Features.VideoCall.StartCall
{
    public class StartCallCommandHandler : IRequestHandler<StartCallCommand, string>
    {
        private readonly IHubContextService _hubContextService;

        public StartCallCommandHandler(IHubContextService hubContextService)
        {
            _hubContextService = hubContextService;
        }

        public async Task<string> Handle(StartCallCommand request, CancellationToken cancellationToken)
        {
            var roomId = Guid.NewGuid();
            return roomId.ToString();
        }
    }
}