using System;

namespace Domain.Services;

public interface IFileUploadService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string uploadPath);
}
