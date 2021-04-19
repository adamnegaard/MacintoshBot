using System;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using MacintoshBot.Models.User;
using MacintoshBot.Tests.Repositories;
using MacintoshBot.XpHandlers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests
{
    public class XpGrantTests
    {
        private readonly IXpGrantModel _xpGrantModel;

        public XpGrantTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();

            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            var userRepository = new UserRepository(context);
            _xpGrantModel = new XpGrantModel(userRepository);
        }

        [Theory]
        [InlineData(-5, 4, 1, 1025)]
        [InlineData(-10, 1, 1, 648)]
        [InlineData(-100, 2, 1, 500)]
        [InlineData(-567, 2, 2, 2835)]
        [InlineData(10, 3, 1, 95)]
        public async Task TestXpGainedCorretly(int minutesAgo, ulong memberId, ulong guildId, int expected)
        {
            var beginTime = DateTime.Now.AddMinutes(minutesAgo);

            var newXp = await _xpGrantModel.GetNewXpFromStartTime(beginTime, memberId, guildId);

            Assert.Equal(expected, newXp);
        }
    }
}