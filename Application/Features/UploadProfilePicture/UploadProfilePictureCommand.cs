using System;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UploadProfilePicture;


public record UploadProfilePictureDto (Guid UserId, IFormFile File): IRequest<string>;
public record UploadProfilePictureCommand(UploadProfilePictureDto UploadProfilePictureRequest): IRequest<Response<string>>;
