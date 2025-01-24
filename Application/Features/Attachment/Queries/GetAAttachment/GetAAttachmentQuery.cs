using System;
using Domain.Enums;
using Domain.Utils;
using MediatR;

namespace Application.Features.Attachment.Queries.GetAAttachment;


public record AttachmentResponseDto(Guid Id,FileType Type, string FilePath, DateTime UploadedAt);


public record GetAAttachmentQuery(Guid AttachmentId) : IRequest<Response<AttachmentResponseDto>>;