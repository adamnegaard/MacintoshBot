using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.Message;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests.Repositories
{
    public class MessageRepositoryTests
    {
        private readonly IMessageRepository _messageRepository;

        public MessageRepositoryTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();

            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            _messageRepository = new MessageRepository(context);
        }

        [Fact]
        public async void GetExisting()
        {
            var message = await _messageRepository.GetMessageId("role", 1);
            Assert.Equal(Status.Found, message.status);
            Assert.Equal(1u, message.messageId);
        }

        [Fact]
        public async void GetOnNonExistingName()
        {
            var message = await _messageRepository.GetMessageId("test", 1);
            Assert.Equal(Status.BadRequest, message.status);
            Assert.Equal(0u, message.messageId);
        }

        [Fact]
        public async void GetOnNonExistingGuild()
        {
            var message = await _messageRepository.GetMessageId("role", 3);
            Assert.Equal(Status.BadRequest, message.status);
            Assert.Equal(0u, message.messageId);
        }

        [Fact]
        public async void CreateOnNonExisting()
        {
            var messageCreate = new MessageDTO
            {
                RefName = "test",
                GuildId = 3,
                MessageId = 5
            };
            var message = await _messageRepository.Create(messageCreate);
            Assert.Equal(Status.Created, message.status);
            Assert.Equal(5u, message.message.MessageId);
        }

        [Fact]
        public async void CreateOnExisting()
        {
            var messageCreate = new MessageDTO
            {
                RefName = "test",
                GuildId = 2,
                MessageId = 5
            };
            var message = await _messageRepository.Create(messageCreate);
            Assert.Equal(Status.Conflict, message.status);
            Assert.Null(message.message);
        }
    }
}