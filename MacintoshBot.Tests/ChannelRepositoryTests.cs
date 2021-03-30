using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.Channel;
using MacintoshBot.Models.Group;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests
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
            var channel = await _channelRepository.Get("role", 1);
            Assert.Equal("role", channel.RefName);
            Assert.Equal(1u, channel.GuildId);
            Assert.Equal(1u, channel.ChannelId);
        }
        
        [Fact]
        public async void GetNonExistingChannelName()
        {
            var channel = await _channelRepository.Get("test", 1);
            Assert.Null(channel);
        }
        
        [Fact]
        public async void GetNonExistingChannelGuild()
        {
            var channel = await _channelRepository.Get("role", 42);
            Assert.Null(channel);
        }
        
        [Fact]
        public async void CreateExisting()
        {
            var channel = new ChannelDTO
            {
                RefName = "role",
                GuildId = 1,
                ChannelId = 1,
            };
            var actual = await _channelRepository.Create(channel);
            Assert.Equal(Status.Conflict, actual);
        }
        
        [Fact]
        public async void CreateNonExistingName()
        {
            var channel = new ChannelDTO
            {
                RefName = "test",
                GuildId = 1,
                ChannelId = 1,
            };
            var actual = await _channelRepository.Create(channel);
            Assert.Equal(Status.Created, actual);
        }
        
        [Fact]
        public async void CreateNonExistingGuild()
        {
            var channel = new ChannelDTO
            {
                RefName = "role",
                GuildId = 42,
                ChannelId = 1,
            };
            var actual = await _channelRepository.Create(channel);
            Assert.Equal(Status.Created, actual);
        }
    }
}