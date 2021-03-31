using System.Linq;
using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.Group;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests.Repositories
{
    public class GroupRepositoryTests
    {
        private readonly IGroupRepository _groupRepository;
        
        public GroupRepositoryTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();
            
            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            _groupRepository = new GroupRepository(context);
        }

        [Fact]
        public async void CheckGroupCountCorrect()
        {
            var groups1 = await _groupRepository.Get(1);
            Assert.Equal(5, groups1.Count());
            
            var groups2 = await _groupRepository.Get(2);
            Assert.Equal(1, groups2.Count());
        }

        [Fact]
        public async void roleIdOnExisting()
        {
            var response = await _groupRepository.GetRoleIdFromEmoji(":wow:", 1);
            Assert.Equal(Status.Found, response.status);
            Assert.Equal(3u, response.roleId);
        }
        
        [Fact]
        public async void roleIdOnNonExistingEmoji()
        {
            var response = await _groupRepository.GetRoleIdFromEmoji(":shouldfail:", 1);
            Assert.Equal(Status.BadRequest, response.status);
            Assert.Equal(0u, response.roleId);
        }
        
        [Fact]
        public async void roleIdOnNonExistingGuild()
        {
            var response = await _groupRepository.GetRoleIdFromEmoji(":wow:", 42);
            Assert.Equal(Status.BadRequest, response.status);
            Assert.Equal(0u, response.roleId);
        }

        [Fact]
        public async void GetOnExisting()
        {
            var response = await _groupRepository.Get("WoW", 1);
            Assert.Equal(Status.Found, response.status);
            var group = response.group;
            Assert.Equal("WoW", group.Name);
            Assert.Equal(1u, group.GuildId);
            Assert.Equal("World of Warcraft", group.FullName);
            Assert.True(group.IsGame);
            Assert.Equal(":wow:", group.EmojiName);
            Assert.Equal(3u, group.DiscordRoleId);
        }
        
        [Fact]
        public async void GetOnNonExistingName()
        {
            var response = await _groupRepository.Get("test", 1);
            Assert.Equal(Status.BadRequest, response.status);
            Assert.Null(response.group);
        }
        
        [Fact]
        public async void GetOnNonExistingGuild()
        {
            var response = await _groupRepository.Get("Wow", 42);
            Assert.Equal(Status.BadRequest, response.status);
            Assert.Null(response.group);
        }
        
        [Fact]
        public async void CreateOnExistingNameAndGuild()
        {
            var groupDTO = new GroupDTO
            {
                Name = "WoW",
                GuildId = 1,
                FullName = "World of Warcraft",
                IsGame = false,
                EmojiName = ":wow:",
                DiscordRoleId = 4,
            };
            var group = await _groupRepository.Create(groupDTO);
            Assert.Equal(Status.Conflict, group.status);
            Assert.Equal("WoW", group.group.Name);
            Assert.Equal(3u, group.group.DiscordRoleId);
        }
        
        [Fact]
        public async void CreateOnExistingName()
        {
            var groupDTO = new GroupDTO
            {
                Name = "WoW",
                GuildId = 3,
                FullName = "World of Warcraft",
                IsGame = false,
                EmojiName = ":wow:",
                DiscordRoleId = 4,
            };
            var group = await _groupRepository.Create(groupDTO);
            Assert.Equal(Status.Created, group.status);
        }
        
        [Fact]
        public async void CreateOnExistingGuild()
        {
            var groupDTO = new GroupDTO
            {
                Name = "Test",
                GuildId = 1,
                FullName = "Test",
                IsGame = false,
                EmojiName = ":wow:",
                DiscordRoleId = 4,
            };
            var group = await _groupRepository.Create(groupDTO);
            Assert.Equal(Status.Created, group.status);
        }
        
        [Fact]
        public async void CreateOnNonExisting()
        {
            var groupDTO = new GroupDTO
            {
                Name = "Test",
                GuildId = 42,
                FullName = "Test",
                IsGame = false,
                EmojiName = ":test:",
                DiscordRoleId = 5,
            };
            var group = await _groupRepository.Create(groupDTO);
            Assert.Equal(Status.Created, group.status);
        }

        [Fact]
        public async void DeleteExistingGroup()
        {
            var status = await _groupRepository.Delete("WoW", 1);
            Assert.Equal(Status.Deleted, status);
        }
        
        [Fact]
        public async void DeleteNonExistingGuild()
        {
            var status = await _groupRepository.Delete("WoW", 42);
            Assert.Equal(Status.BadRequest, status);
        }
        
        [Fact]
        public async void DeleteNonExistingName()
        {
            var status = await _groupRepository.Delete("test", 1);
            Assert.Equal(Status.BadRequest, status);
        }
    }
}