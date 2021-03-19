using System;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Entities
{
    public class DiscordContext : DbContext, IDiscordContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Role> LevelRoles { get; set; }

        public DiscordContext(DbContextOptions<DiscordContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity
                    .HasKey(u => u.UserId);
                
                entity
                    .HasIndex(u => u.UserId)
                    .IsUnique();

                entity.HasData(new[]
                {
                    new User
                    {
                        UserId = 234395307759108106,
                        Xp = 0
                    },
                    new User
                    {
                        UserId = 809373068676956220,
                        Xp = 0
                    },
                    new User
                    {
                        UserId = 255367450227507202,
                        Xp = 0
                    },
                    new User
                    {
                        UserId = 235025980073181184,
                        Xp = 0
                    },
                    new User
                    {
                        UserId = 267259924973748224,
                        Xp = 0
                    },
                    new User
                    {
                        UserId = 240570596105125889,
                        Xp = 0
                    },
                    new User
                    {
                        UserId = 296642397503488000,
                        Xp = 95
                    },
                    new User
                    {
                        UserId = 261086354828427264,
                        Xp = 0
                    },
                });
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity
                    .HasKey(i => i.Title);
                
                entity
                    .HasIndex(i => i.Title)
                    .IsUnique();

                entity.HasData(new[]
                {
                    new Image
                    {
                        Location = new Uri("http://themacs.dk/DiscordImages/poggers.jpg"),
                        Title = "poggers"
                    },
                    new Image
                    {
                        Location = new Uri("http://themacs.dk/DiscordImages/big-spender.jpg"),
                        Title = "big-spender"
                    },
                });
            });
            
            modelBuilder.Entity<Group>(entity =>
            {
                entity
                    .HasKey(e => e.Name);

                entity
                    .HasIndex(e => e.Name)
                    .IsUnique();

                entity
                    .HasOne(g => g.DiscordRole)
                    .WithOne(); 
            });
            
            modelBuilder.Entity<Message>(entity =>
            {
                entity
                    .HasKey(m => m.DiscordId);
                
                entity
                    .HasIndex(m => m.DiscordId)
                    .IsUnique();

                entity.HasData(new[]
                {
                    new Message
                    {
                        DiscordId = 810921052939091988,
                        RefName = "role"
                    }
                });
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity
                    .HasKey(r => r.RefName);
                
                entity
                    .HasIndex(r => r.RefName)
                    .IsUnique();
                
                entity
                    .HasOne(r => r.DiscordRole)
                    .WithOne(); 
            });
        }
    }
}