using System;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using MacintoshBot.Models.User;
using MacintoshBot.Models.VoiceState;
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
            var voiceStateRepository = new VoiceStateRepository(context, null);
            _xpGrantModel = new XpGrantModel(userRepository, voiceStateRepository, null);
        }

        // TODO TEST
    }
}