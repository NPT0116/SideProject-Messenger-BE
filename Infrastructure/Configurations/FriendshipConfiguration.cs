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
    internal class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>

    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(fr => fr.Id);

            builder.HasOne(f => f.Initiator)
                .WithMany(u => u.FriendshipsInitiated)
                .HasForeignKey(f => f.InitiatorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // User2 relationship (Recipient)
            builder.HasOne(f => f.Receiver)
                .WithMany(u => u.FriendshipsReceived)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
