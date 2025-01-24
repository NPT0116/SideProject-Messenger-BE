using System;
using Application.Features.Attachment.Queries.GetAAttachment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Attachment;

[Route("api/attachment")]
[ApiController]
public class AttachmentController: ControllerBase
{
    private readonly IMediator _mediator;

    public AttachmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAAttachment(Guid id)
    {
        var response = await _mediator.Send(new GetAAttachmentQuery(id));

        return Ok(response);
    }
}
