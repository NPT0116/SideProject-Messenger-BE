using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Attachment> GetById(Guid id)
        {
            return await _context.Attachments.FindAsync(id);
        }

        public async Task<Attachment> Create(Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            attachment.Id = Guid.NewGuid();
            attachment.UploadedAt = DateTime.UtcNow;

            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task<Attachment> Update(Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            _context.Attachments.Attach(attachment);
            _context.Entry(attachment).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task<Attachment> Delete(Guid id)
        {
            var attachment = await _context.Attachments.FindAsync(id);

            if (attachment == null)
            {
                return null;
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }
    }
}
