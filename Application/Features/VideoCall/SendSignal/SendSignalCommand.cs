using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.VideoCall.SendSignal;

public record SendSignalCommand
    (string connectionId, 
    string roomId, 
    string userId, 
    string signal) 
        : IRequest;