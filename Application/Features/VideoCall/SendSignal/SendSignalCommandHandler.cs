using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.VideoCall.SendSignal
{
    public class SendSignalCommandHandler : IRequestHandler<SendSignalCommand>
    {
        public Task Handle(SendSignalCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}