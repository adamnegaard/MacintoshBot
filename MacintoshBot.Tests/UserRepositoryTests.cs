using System.Linq;
using MacintoshBot.Entities;
using MacintoshBot.Models.User;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests
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
            var allUsers = await _userRepository.GetAll();
            Assert.Equal(6, allUsers.Count());
            
            var guildUsers = await _userRepository.Get(1);
            Assert.Equal(5, guildUsers.Count());
        }
    }
}