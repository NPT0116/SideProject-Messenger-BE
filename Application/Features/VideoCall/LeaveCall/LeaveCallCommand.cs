using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.VideoCall.LeaveCall;

public record LeaveCallCommand(
    string connectionId,
    string roomId,
    string userId
) : IRequest;
