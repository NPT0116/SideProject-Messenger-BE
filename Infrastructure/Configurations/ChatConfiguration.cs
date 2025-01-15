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
    internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasMany(c => c.Participants)
                .WithOne()
                .HasForeignKey(p => p.ChatId);

            builder.HasMany(c => c.Messages)
                .WithOne()
                .HasForeignKey(p => p.ChatId);


        }
    }
}
