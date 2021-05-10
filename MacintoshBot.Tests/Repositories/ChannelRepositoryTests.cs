using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.Channel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests.Repositories
{
    public class ChannelRepositoryTests
    {
        private readonly IChannelRepository _channelRepository;

        public ChannelRepositoryTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();

            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            _channelRepository = new ChannelRepository(context);
        }

        [Fact]
        public async void GetExistingChannel()
        {
            var response = await _channelRepository.Get("role", 1);
            Assert.Equal(Status.Found, response.status);
            Assert.Equal("role", response.channel.RefName);
            Assert.Equal(1u, response.channel.GuildId);
            Assert.Equal(1u, response.channel.ChannelId);
        }

        [Fact]
        public async void GetNonExistingChannelName()
        {
            var response = await _channelRepository.Get("test", 1);
            Assert.Equal(Status.BadRequest, response.status);
            Assert.Null(response.channel);
        }

        [Fact]
        public async void GetNonExistingChannelGuild()
        {
            var response = await _channelRepository.Get("role", 42);
            Assert.Equal(Status.BadRequest, response.status);
            Assert.Null(response.channel);
        }

        [Fact]
        public async void CreateExisting()
        {
            var channel = new ChannelDTO
            {
                RefName = "role",
                GuildId = 1,
                ChannelId = 1
            };
            var actual = await _channelRepository.Create(channel);
            Assert.Equal(Status.Conflict, actual.status);
            Assert.Equal("role", actual.channel.RefName);
            Assert.Equal(1u, actual.channel.GuildId);
            Assert.Equal(1u, actual.channel.ChannelId);
        }

        [Fact]
        public async void CreateNonExistingName()
        {
            var channel = new ChannelDTO
            {
                RefName = "test",
                GuildId = 1,
                ChannelId = 1
            };
            var actual = await _channelRepository.Create(channel);
            Assert.Equal(Status.Created, actual.status);
        }

        [Fact]
        public async void CreateNonExistingGuild()
        {
            var channel = new ChannelDTO
            {
                RefName = "role",
                GuildId = 42,
                ChannelId = 1
            };
            var actual = await _channelRepository.Create(channel);
            Assert.Equal(Status.Created, actual.status);
        }
    }
}