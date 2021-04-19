using System.Linq;
using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.File;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests.Repositories
{
    public class FileRepositoryTests
    {
        private readonly IFileRepository _fileRepository;

        public FileRepositoryTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();

            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            _fileRepository = new FileRepository(context, null);
        }

        [Fact]
        public async void GetLocationOnExisting()
        {
            var response = await _fileRepository.Get("poggers", 1);
            Assert.Equal(Status.Found, response.status);
            Assert.Equal("http://test/", response.file.Location);
        }

        [Fact]
        public async void GetLocationOnNonExistingName()
        {
            var response = await _fileRepository.Get("pawg", 1);
            Assert.Equal(Status.BadRequest, response.status);
        }

        [Fact]
        public async void CreateOnNonExisting()
        {
            var file = new FileDTO
            {
                Title = "test",
                GuildId = 2,
                Location = "http://test3/"
            };
            var response = await _fileRepository.Create(file);
            Assert.Equal(Status.Created, response.status);
            var createdFile = response.file;

            Assert.Equal("test", createdFile.Title);
            Assert.Equal(2u, createdFile.GuildId);
            Assert.Equal("http://test3/", createdFile.Location);
        }

        [Fact]
        public async void CreateOnExisting()
        {
            var file = new FileDTO
            {
                Title = "poggers",
                GuildId = 1,
                Location = "http://test3/"
            };
            var response = await _fileRepository.Create(file);
            Assert.Equal(Status.Conflict, response.status);
        }

        [Fact]
        public async void GetLocationOnNonExistingGuild()
        {
            var response = await _fileRepository.Get("poggers", 2);
            Assert.Equal(Status.BadRequest, response.status);
        }

        [Fact]
        public async void CheckImageCountCorrect()
        {
            var images = await _fileRepository.Get(1);
            Assert.Equal(2, images.Count());
        }

        [Fact]
        public async void CheckImageCountCorrectInNonExistingGuild()
        {
            var images = await _fileRepository.Get(2);
            Assert.False(images.Any());
        }
    }
}