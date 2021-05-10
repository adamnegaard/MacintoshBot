using System.Linq;
using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();

            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            _userRepository = new UserRepository(context);
        }

        [Fact]
        public async void CheckUserCountCorrect()
        {
            var allUsers = await _userRepository.Get();
            Assert.Equal(6, allUsers.Count());
        }

        [Fact]
        public async void CheckUserCountCorrectInGuild()
        {
            var guildUsers = await _userRepository.Get(1);
            Assert.Equal(5, guildUsers.Count());
        }

        [Fact]
        public async void CreateExistingUser()
        {
            var user = await _userRepository.Create(1, 1);
            Assert.Equal(Status.Conflict, user.status);
            Assert.Equal(1u, user.user.UserId);
            Assert.Equal(598, user.user.Xp);
        }

        [Fact]
        public async void CreateNonExistingUserDifferentGuild()
        {
            var user = await _userRepository.Create(1, 3);
            Assert.Equal(Status.Created, user.status);
        }

        [Fact]
        public async void CreateNonExistingUserSameGuild()
        {
            var user = await _userRepository.Create(42, 1);
            Assert.Equal(Status.Created, user.status);
        }

        [Fact]
        public async void DeleteNonExistingUser()
        {
            var actual = await _userRepository.Delete(42, 1);
            Assert.Equal(Status.BadRequest, actual);
        }

        [Fact]
        public async void DeleteExistingUser()
        {
            var actual = await _userRepository.Delete(1, 1);
            Assert.Equal(Status.Deleted, actual);
        }

        [Fact]
        public async void GetExistingUser()
        {
            var result = await _userRepository.Get(1, 1);
            Assert.Equal(Status.Found, result.status);
            var user = result.user;

            Assert.Equal(1u, user.UserId);
            Assert.Equal(1u, user.GuildId);
            Assert.Equal(598, user.Xp);
            Assert.Equal(5, user.Level);
        }

        [Fact]
        public async void GetNonExistingUser()
        {
            var user = await _userRepository.Get(42, 1);
            Assert.Equal(Status.BadRequest, user.status);
        }

        [Fact]
        public async void AddXpExistingUser()
        {
            var actual = await _userRepository.AddXp(1, 1, 2);
            Assert.Equal(600, actual);
        }

        [Fact]
        public async void AddXpNonExistingUser()
        {
            var actual = await _userRepository.AddXp(42, 1, 20);
            Assert.Equal(0, actual);
        }
    }
}