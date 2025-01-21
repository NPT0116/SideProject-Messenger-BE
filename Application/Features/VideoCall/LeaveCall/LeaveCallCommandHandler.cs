using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.VideoCall.LeaveCall
{
    public class LeaveCallCommandHandler : IRequestHandler<LeaveCallCommand>
    {
        public Task Handle(LeaveCallCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}