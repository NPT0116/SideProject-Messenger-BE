using System;
using Domain.Services;

namespace Infrastructure.Services;

public class FileUploadService : IFileUploadService
{
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string uploadPath)
    {
        string unqieFileName = Guid.NewGuid() + Path.GetExtension(fileName);
        string filePath = Path.Combine(uploadPath, unqieFileName);

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
          using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        return unqieFileName;
    }
}
