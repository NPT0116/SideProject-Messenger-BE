using Domain.Entities;
using Infrastructure.Identity;
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

            builder.Property(fr => fr.InitiatorId)
                .IsRequired();

            builder.Property(fr => fr.ReceiverId)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.FriendshipsInitiated)
                .HasForeignKey(p => p.InitiatorId)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.FriendshipsReceived)
                .HasForeignKey(p => p.ReceiverId)
                .IsRequired();
        }
    }
}
