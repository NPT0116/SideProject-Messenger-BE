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
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(u => u.FriendshipsInitiated) // One-to-many relationship
                .WithOne(fri => fri.Initiator)          // Navigation property in Friendship
                .HasForeignKey(fri => fri.InitiatorId); // Foreign key in Friendship

            builder.HasMany(u => u.FriendshipsReceived) // One-to-many relationship
                .WithOne(f => f.Receiver)          // Navigation property in Friendship
                .HasForeignKey(f => f.ReceiverId); // Foreign key in Friendship
        }
    }
}
