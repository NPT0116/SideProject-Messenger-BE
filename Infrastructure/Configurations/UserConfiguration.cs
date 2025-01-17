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
    internal class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(u => u.FriendshipsInitiated) // One-to-many relationship 
                .WithOne()                      // Navigation property in Friendship
                .HasForeignKey(fri => fri.InitiatorId); // Foreign key in Friendship

            builder.HasMany(u => u.FriendshipsReceived) // One-to-many relationship
                .WithOne()          // Navigation property in Friendship
                .HasForeignKey(f => f.ReceiverId); // Foreign key in Friendship

            builder.HasMany(u => u.Participants)
                .WithOne()
                .HasForeignKey(f => f.UserId);
        }
    }
}
