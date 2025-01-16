using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Chat)
            .WithMany(c => c.Messages) // Assuming Chat has a collection of Messages
            .HasForeignKey(m => m.ChatId) // Add a ChatId property if not present
            .OnDelete(DeleteBehavior.Cascade); // Optional: Define delete behavior

            // Configure the relationship with Sender (Participant)
            builder.HasOne(m => m.Sender)
                .WithMany(p => p.Messages) // Assuming Participant has a collection of Messages
                .HasForeignKey(m => m.SenderId) // Add a SenderId property if not present
                .OnDelete(DeleteBehavior.Restrict); // Optional: Define delete behavior

            // Configure the relationship with Attachment (Optional)
            builder.HasOne(m => m.Attachment)
                .WithOne()
                .HasForeignKey<Message>(m => m.AttachmentId); // Add an AttachmentId property if not present

            // Configure the relationship with Reactions (One-to-Many)
            builder.HasMany(m => m.Reactions)
                .WithOne(r => r.Message) // Assuming Reaction has a navigation property to Message
                .HasForeignKey(r => r.MessageId) // Add a MessageId property in Reaction
                .OnDelete(DeleteBehavior.Cascade); // Optional: Define delete behavior
        }
    }
}

//public Chat Chat { get; private set; }
//public Participant Sender { get; private set; }
//public Attachment? Attachment { get; private set; }
//public IReadOnlyCollection<Reaction> Reactions { get; private set; }