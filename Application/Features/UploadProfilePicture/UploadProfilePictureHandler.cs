using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.File;
using Domain.Exceptions.Users;
using Domain.Repositories;
using Domain.Services;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;

namespace Application.Features.UploadProfilePicture;

public class UploadProfilePictureHandler : IRequestHandler<UploadProfilePictureCommand, Response<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IWebHostEnvironment _environment;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IFileUploadService _fileUploadService;

    public UploadProfilePictureHandler(IUserRepository userRepository, IWebHostEnvironment environment, IAttachmentRepository attachmentRepository, IFileUploadService fileUploadService)
    {
        _userRepository = userRepository;
        _environment = environment;
        _attachmentRepository = attachmentRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<Response<string>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UploadProfilePictureRequest.UserId;
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFound(userId);
        }

        var file = request.UploadProfilePictureRequest.File;
        if (file == null || file.Length == 0)
        {
            throw new EmptyFile();
        }
        if (string.IsNullOrEmpty(_environment.WebRootPath))
        {
            _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // throw new InvalidOperationException("WebRootPath is not set.");
        }
        // Use the FileUploadService to save the file
        var fileName = Path.GetFileName(file.FileName);
        var uploadPath = Path.Combine(_environment.WebRootPath);
        
        var filePath = await _fileUploadService.UploadImageAsync(file.OpenReadStream(), fileName, uploadPath);
        Console.WriteLine(filePath);
        // Create attachment entity
        var attachment = new Domain.Entities.Attachment
        {
            Id = Guid.NewGuid(),
            FilePath = filePath,
            Type = FileType.Image, // Assuming it's an image
            UploadedAt = DateTime.UtcNow
        };

        // Save attachment to repository
        var savedAttachment = await _attachmentRepository.CreateAsync(attachment);

        // Optionally, update the user's profile picture URL in the database
        user.ProfilePictureId = savedAttachment.Id;
        await _userRepository.UpdateUserAsync(user);

        return new Response<string>(user.ProfilePictureId.ToString());
    }
}
