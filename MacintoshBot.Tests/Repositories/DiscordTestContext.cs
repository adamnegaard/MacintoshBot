using System;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Tests
{
    public class DiscordTestContext : DiscordContext
    {
        public DiscordTestContext(DbContextOptions<DiscordContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>(entity =>
            {
                entity
                    .HasData(new[]
                    {
                        new User
                        {
                            UserId = 1,
                            GuildId = 1,
                            Xp = 598,
                        },
                        new User
                        {
                            UserId = 2,
                            GuildId = 1,
                            Xp = 0,
                        },
                        new User
                        {
                            UserId = 3,
                            GuildId = 1,
                            Xp = 95,
                        },
                        new User
                        {
                            UserId = 4,
                            GuildId = 1,
                            Xp = 1000,
                        },
                        new User
                        {
                            UserId = 5,
                            GuildId = 1,
                            Xp = 3,
                        },
                        new User
                        {
                            UserId = 2,
                            GuildId = 2,
                            Xp = 0,
                        },
                    });
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity
                    .HasData(new[]
                    {
                        new Image
                        {
                            Title = "poggers",
                            GuildId = 1,
                            Location = new Uri("http://test/"),
                        },
                        new Image
                        {
                            Title = "big-spender",
                            GuildId = 1,
                            Location = new Uri("http://test2/"),
                        },
                    });
            });
            
            modelBuilder.Entity<Group>(entity =>
            {
                entity
                    .HasData(new[]
                    {
                        new Group
                        {
                            Name = "Rust",
                            GuildId = 1,
                            FullName = "Rust",
                            IsGame = true,
                            EmojiName = ":rust:",
                            DiscordRoleId = 1,
                        },
                        new Group
                        {
                            Name = "Rust",
                            GuildId = 2,
                            FullName = "Rust",
                            IsGame = true,
                            EmojiName = ":rust:",
                            DiscordRoleId = 2,
                        },
                        new Group
                        {
                            Name = "WoW",
                            GuildId = 1,
                            FullName = "World of Warcraft",
                            IsGame = true,
                            EmojiName = ":wow:",
                            DiscordRoleId = 3,
                        },
                        new Group
                        {
                            Name = "LoL",
                            GuildId = 1,
                            FullName = "League of Legends",
                            IsGame = true,
                            EmojiName = ":lol:",
                            DiscordRoleId = 4,
                        },
                        new Group
                        {
                            Name = "SWU",
                            GuildId = 1,
                            FullName = "Software Development",
                            IsGame = false,
                            EmojiName = ":computer:",
                            DiscordRoleId = 5,
                        },
                        new Group
                        {
                            Name = "TheCrew",
                            GuildId = 1,
                            FullName = "The Crew",
                            IsGame = false,
                            EmojiName = ":gun:",
                            DiscordRoleId = 6,
                        },
                    });
            });
            
            modelBuilder.Entity<Message>(entity =>
            {
                entity
                    .HasData(new[]
                    {
                        new Message
                        {
                            RefName = "role",
                            GuildId = 1,
                            MessageId = 1,
                        },
                        new Message
                        {
                            RefName = "test",
                            GuildId = 2,
                            MessageId = 2,
                        },
                    });
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity
                    .HasData(new[]
                    {
                        new Role
                        {
                            RefName = "scrub",
                            GuildId = 1,
                            RoleId = 1,
                            Rank = 0
                        },
                        new Role
                        {
                            RefName = "Intermediate",
                            GuildId = 1,
                            RoleId = 2,
                            Rank = 1
                        },
                        new Role
                        {
                            RefName = "pro",
                            GuildId = 1,
                            RoleId = 3,
                            Rank = 2
                        },
                        new Role
                        {
                            RefName = "test1",
                            GuildId = 2,
                            RoleId = 4,
                            Rank = 0
                        },
                        new Role
                        {
                            RefName = "test2",
                            GuildId = 2,
                            RoleId = 5,
                            Rank = 1
                        },
                    });
            });
            modelBuilder.Entity<Channel>(entity =>
            {
                entity
                    .HasData(new[]
                    {
                        new Channel
                        {
                            RefName = "role",
                            GuildId = 1,
                            ChannelId = 1,
                        },
                        new Channel
                        {
                            RefName = "newmembers",
                            GuildId = 1,
                            ChannelId = 2,
                        },
                        new Channel
                        {
                            RefName = "dailyfacts",
                            GuildId = 1,
                            ChannelId = 3,
                        },
                        new Channel
                        {
                            RefName = "test1",
                            GuildId = 2,
                            ChannelId = 3,
                        },
                        new Channel
                        {
                            RefName = "test2",
                            GuildId = 2,
                            ChannelId = 4,
                        },
                    });
            });
            modelBuilder.Entity<Fact>(entity =>
            {
                entity
                    .HasData(new[]
                    {
                        new Fact
                        {
                            Id = 1,
                            Text = "Fun fact 1",
                        },
                        new Fact
                        {
                            Id = 2,
                            Text = "Fun fact 2",
                        },
                        new Fact
                        {
                            Id = 3,
                            Text = "Fun fact 3",
                        }, 
                    });
            }); 
        }
    }
}