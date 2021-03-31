using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.Facts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests.Repositories
{
    public class FactRepositoryTests
    {
        private readonly IFactRepository _factRepository;
        
        public FactRepositoryTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();
            
            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            _factRepository = new FactRepository(context);
        }

        [Fact]
        public async void GetOnExisting()
        {
            var response = await _factRepository.Get(1);
            Assert.Equal(Status.Found, response.status);
            Assert.Equal("Fun fact 1", response.fact.Text);
            Assert.Equal(1, response.fact.Id);
        }
        
        [Fact]
        public async void GetMostRecent()
        {
            var response = await _factRepository.Get(0);
            Assert.Equal(Status.Found, response.status);
            Assert.Equal("Fun fact 3", response.fact.Text);
            Assert.Equal(3, response.fact.Id);
        }
        
        [Fact]
        public async void Create()
        {
            var fact = await _factRepository.Create("Fun fact 4");
            Assert.Equal(Status.Created,fact.status);
            Assert.Equal("Fun fact 4", fact.fact.Text);
            Assert.Equal(4, fact.fact.Id);
        }
        
    }
}