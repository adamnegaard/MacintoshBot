﻿// <auto-generated />
using System;
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
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            modelBuilder.Entity("MacintoshBot.Entities.Channel", b =>
                {
                    b.Property<string>("RefName")
                        .HasColumnType("text")
                        .HasColumnName("ref_name");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("channel_id");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_ts");

                    b.HasKey("RefName", "GuildId");

                    b.ToTable("channels");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Fact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_ts");

                    b.Property<string>("Text")
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.HasKey("Id");

                    b.ToTable("facts");
                });

            modelBuilder.Entity("MacintoshBot.Entities.File", b =>
                {
                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_ts");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("location");

                    b.HasKey("Title", "GuildId");

                    b.ToTable("files");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Group", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_ts");

                    b.Property<decimal>("DiscordRoleId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("discord_role_id");

                    b.Property<string>("EmojiName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("emoji_name");

                    b.Property<string>("FullName")
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<bool>("IsGame")
                        .HasColumnType("boolean")
                        .HasColumnName("is_game");

                    b.HasKey("Name", "GuildId");

                    b.ToTable("groups");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Message", b =>
                {
                    b.Property<string>("RefName")
                        .HasColumnType("text")
                        .HasColumnName("ref_name");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_ts");

                    b.Property<decimal>("MessageId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("message_id");

                    b.HasKey("RefName", "GuildId");

                    b.ToTable("messages");
                });

            modelBuilder.Entity("MacintoshBot.Entities.Role", b =>
                {
                    b.Property<string>("RefName")
                        .HasColumnType("text")
                        .HasColumnName("ref_name");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_ts");

                    b.Property<int>("Rank")
                        .HasColumnType("integer")
                        .HasColumnName("rank");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("role_id");

                    b.HasKey("RefName", "GuildId");

                    b.ToTable("roles");
                });

            modelBuilder.Entity("MacintoshBot.Entities.User", b =>
                {
                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_ts");

                    b.Property<decimal?>("SteamId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("steam_id");

                    b.Property<string>("SummonerName")
                        .HasColumnType("text")
                        .HasColumnName("summoner_name");

                    b.Property<int>("Xp")
                        .HasColumnType("integer")
                        .HasColumnName("xp");

                    b.HasKey("UserId", "GuildId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("MacintoshBot.Entities.VoiceState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("EnteredTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("entered_ts");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime?>("LeftTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("left_ts");

                    b.Property<DateTime?>("MovedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("moved_time");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.ToTable("voice_states");
                });
#pragma warning restore 612, 618
        }
    }
}
