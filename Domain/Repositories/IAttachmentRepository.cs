using System;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAttachmentRepository
{
    public Task<Attachment> GetByIdAsync(Guid id);
    public Task<Attachment> CreateAsync(Attachment attachment);
    public Task<Attachment> UpdateAsync(Attachment attachment);
    public Task<Attachment> DeleteAsync(Guid id);
}
