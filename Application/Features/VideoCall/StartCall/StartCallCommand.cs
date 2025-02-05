using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.VideoCall.StartCall;

public record StartCallCommand(string friendId) : IRequest<string>;
