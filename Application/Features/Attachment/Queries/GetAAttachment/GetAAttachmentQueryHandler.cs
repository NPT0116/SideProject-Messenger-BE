using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Features.Attachment.Queries.GetAAttachment
{
    public class GetAAttachmentQueryHandler : IRequestHandler<GetAAttachmentQuery, Response<AttachmentResponseDto>>
    {
        private readonly IAttachmentRepository _attachmentRepository;

        public GetAAttachmentQueryHandler(IAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        public async Task<Response<AttachmentResponseDto>> Handle(GetAAttachmentQuery request, CancellationToken cancellationToken)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(request.AttachmentId);

            if (attachment == null)
            {
                throw new FileNotFoundException("Attachment not found");
            }

            var attachmentDto = new AttachmentResponseDto(
                attachment.Id,
                attachment.Type,
                attachment.FilePath,
                attachment.UploadedAt
            );

            return new Response<AttachmentResponseDto>(attachmentDto);
        }
    }
}