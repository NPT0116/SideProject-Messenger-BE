using System;
using Domain.Entities;

namespace Domain.Repositories;

public interface IAttachmentRepository
{
    public Task<Attachment> GetById(Guid id);
    public Task<Attachment> Create(Attachment attachment);
    public Task<Attachment> Update(Attachment attachment);
    public Task<Attachment> Delete(Guid id);
}
