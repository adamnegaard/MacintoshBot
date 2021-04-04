using System;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Entities
{
    public class DiscordContext : DbContext, IDiscordContext
    {
        public DbSet<User> Members { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Role> LevelRoles { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Fact> Facts { get; set; }

        public DiscordContext(DbContextOptions<DiscordContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                //Composite key between the user and the guild
                entity
                    .HasKey(u => new {u.UserId, u.GuildId});
            });

            modelBuilder.Entity<File>(entity =>
            {
                //Composite key between the file title and the guild
                entity
                    .HasKey(i => new {i.Title, i.GuildId});
            });
            
            modelBuilder.Entity<Group>(entity =>
            {
                //Composite key between the name and the guild
                entity
                    .HasKey(e => new {e.Name, e.GuildId});
            });
            
            modelBuilder.Entity<Message>(entity =>
            {
                //Composite key between the message and the guild
                entity
                    .HasKey(m => new {m.RefName, m.GuildId});
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity
                    .HasKey(r => new {r.RefName, r.GuildId});
            });
            modelBuilder.Entity<Channel>(entity =>
            {
                entity
                    .HasKey(r => new {r.RefName, r.GuildId});
            });
            modelBuilder.Entity<Fact>(entity =>
            {
                entity
                    .HasKey(f => f.Id);
            });
            
            //For serial keys
            modelBuilder.UseSerialColumns();
        }
    }
}