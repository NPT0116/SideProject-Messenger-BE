using Domain.Entities;
using Domain.Enums;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Seed
{
    public class DatabaseSeed
    {
        public static async Task SeedData(ApplicationDbContext context)
        {
            Console.WriteLine("Seedddddddddddddddddddddddddddddddd");
            Console.WriteLine(context.Users.Count());
            // Seed Users
            if (!context.Users.Any())
            {
                Console.WriteLine("Run seed for users");
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                string defaultPassword = "123456";
                string hashedPassword = passwordHasher.HashPassword(null, defaultPassword);
                var users = Enumerable.Range(1, 5000).Select(i =>
                {
                    var user = new ApplicationUser(
                       
                        Guid.NewGuid(),
                        $"FirstName{i}",
                        $"LastName{i}"
                    ){
                        UserName = $"user{i}",
                        PasswordHash = hashedPassword
                    };
                    return user;
                }).ToList();

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
            Console.WriteLine("Finish seed for uses");
            // Seed Attachments
            // if (!context.Attachments.Any())
            // {
            //     var attachments = Enumerable.Range(1, 5000).Select(i => new Attachment
            //     {
            //         Id = Guid.NewGuid(),
            //         Type = (FileType)(i % 3), // Assume 3 FileTypes
            //         FilePath = $"/uploads/file_{i}.png",
            //         UploadedAt = DataGenerator.RandomDate(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow)
            //     }).ToList();

            //     await context.Attachments.AddRangeAsync(attachments);
            // }

            // Seed Chats
            if (!context.Chats.Any())
            {
                var chats = Enumerable.Range(1, 5000).Select(i => new Chat(
                    Guid.NewGuid(),
                    (ChatType)(i % 2) // Assume 2 ChatTypes
                )).ToList();

                await context.Chats.AddRangeAsync(chats);
                await context.SaveChangesAsync();
            }

            // Seed Friendships
            if (!context.Friendships.Any())
            {
                var userIds = context.Users.Select(u => u.Id).ToList();
                var friendships = Enumerable.Range(1, 5000)
                        .Select(i => new Friendship(
                            Guid.NewGuid(), 
                            userIds[i % userIds.Count], 
                            userIds[(i + 1) % userIds.Count]))
                        .ToList();

                await context.Friendships.AddRangeAsync(friendships);
                await context.SaveChangesAsync();
            }

            if (!context.Participants.Any())
            {
                var chatIds = context.Chats.Select(c => c.Id).ToList();
                var userIds = context.Users.Select(u => u.Id).ToList();

                var participants = Enumerable
                    .Range(1, 5000)
                    .Select(i => new Participant(
                        Guid.NewGuid(),
                        userIds[i % userIds.Count],
                        chatIds[i % chatIds.Count])).ToList();

                await context.Participants.AddRangeAsync(participants);
                await context.SaveChangesAsync();
            }

            // Seed Messages
            if (!context.Messages.Any())
            {
                var chatIds = context.Chats.Select(c => c.Id).ToList();
                var userIds = context.Participants.Select(u => u.Id).ToList();
                var attachments = context.Attachments.Select(a => a.Id).ToList();

                var messages = Enumerable
                    .Range(1, 5000)
                    .Select(i => new Message(
                        Guid.NewGuid(),
                        DataGenerator.RandomString(100),
                        (MessageType)(i % 3),
                        userIds[i % userIds.Count],
                        chatIds[i % chatIds.Count])).ToList();

                await context.Messages.AddRangeAsync(messages);
                await context.SaveChangesAsync();
            }

            // Seed Reactions
            if (!context.Reactions.Any())
            {
                var messageIds = context.Messages.Select(m => m.Id).ToList();

                var possibleReactions = new List<string>
                {
                    "👍",  // Thumbs Up
                    "❤️",  // Heart
                    "😂",  // Laughing
                    "😮",  // Wow
                    "😢",  // Sad
                    "😡"   // Angry
                };

                var random = new Random();

                var reactions = Enumerable
                    .Range(1, 5000)
                    .Select(i => new Reaction(
                        Guid.NewGuid(),
                        possibleReactions[random.Next(possibleReactions.Count)],
                        messageIds[random.Next(messageIds.Count)])).ToList();

                await context.Reactions.AddRangeAsync(reactions);
            }

            await context.SaveChangesAsync();
        }
    }
}