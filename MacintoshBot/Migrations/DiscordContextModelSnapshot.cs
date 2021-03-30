﻿// <auto-generated />
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MacintoshBot.Migrations
{
    [DbContext(typeof(DiscordContext))]
    partial class DiscordContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            modelBuilder.Entity("MacintoshBot.Entities.Channel", b =>
                {
                    b.Property<string>("RefName")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("RefName", "GuildId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Fact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Facts");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Group", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("DiscordRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("EmojiName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<bool>("IsGame")
                        .HasColumnType("boolean");

                    b.HasKey("Name", "GuildId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Image", b =>
                {
                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Title", "GuildId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Message", b =>
                {
                    b.Property<string>("RefName")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("MessageId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("RefName", "GuildId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Role", b =>
                {
                    b.Property<string>("RefName")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("Rank")
                        .HasColumnType("integer");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("RefName", "GuildId");

                    b.ToTable("LevelRoles");
                });

            modelBuilder.Entity("MacintoshBot.Entities.User", b =>
                {
                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("Xp")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "GuildId");

                    b.ToTable("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
