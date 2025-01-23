using System;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Users.Queries.GetUserFromParticipantId;

public record GetUserFromParticipantIdQuery(Guid ParticipantId): IRequest<Response<User>>;