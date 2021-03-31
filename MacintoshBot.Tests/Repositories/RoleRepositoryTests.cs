using System;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using MacintoshBot.Models;
using MacintoshBot.Models.Role;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MacintoshBot.Tests.Repositories
{
    public class RoleRepositoryTests
    {
        private readonly ILevelRoleRepository _roleRepository;
        
        public RoleRepositoryTests()
        {
            //Connection
            var connection = new SqliteConnection("datasource=:memory:");
            connection.Open();
            
            //Context
            var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
            var context = new DiscordTestContext(builder.Options);
            context.Database.EnsureCreated();

            _roleRepository = new LevelRoleRepository(context);
        }

        [Fact]
        public async void GetOnExistingByName()
        {
            var response = await _roleRepository.Get("scrub", 1);
            Assert.Equal(Status.Found, response.status);
            var role = response.role;
            Assert.Equal("scrub", role.RefName);
            Assert.Equal(1u, role.GuildId);
            Assert.Equal(1u, role.RoleId);
            Assert.Equal(0, role.Rank);
        }
        
        [Fact]
        public async void GetOnNonExistingGuildByName()
        {
            var response = await _roleRepository.Get("scrub", 42);
            Assert.Equal(Status.BadRequest, response.status);
            var role = response.role;
            Assert.Null(role);
        }
        
        [Fact]
        public async void GetOnNonExistingNameByName()
        {
            var response = await _roleRepository.Get("test47", 1);
            Assert.Equal(Status.BadRequest, response.status);
            var role = response.role;
            Assert.Null(role);
        }

        [Fact]
        public async void GetOnExistingByRoleId()
        {
            var response = await _roleRepository.Get(1, 1);
            Assert.Equal(Status.Found, response.status);
            var role = response.role;
            Assert.Equal("scrub", role.RefName);
            Assert.Equal(1u, role.GuildId);
            Assert.Equal(1u, role.RoleId);
            Assert.Equal(0, role.Rank);
        }
        
        [Fact]
        public async void GetOnNonExistingRoleByRoleId()
        {
            var response = await _roleRepository.Get(42, 1);
            Assert.Equal(Status.BadRequest, response.status);
            var role = response.role;
            Assert.Null(role);
        }
        
        [Fact]
        public async void GetOnNonExistingGuildByRoleId()
        {
            var response = await _roleRepository.Get(1, 42);
            Assert.Equal(Status.BadRequest, response.status);
            var role = response.role;
            Assert.Null(role);
        }

        [Fact]
        public async void CreateOnExisting()
        {
            var roleDTO = new RoleDTO
            {
                RefName = "scrub",
                GuildId = 1,
                RoleId = 1,
                Rank = 0
            };
            var response = await _roleRepository.Create(roleDTO);
            Assert.Equal(Status.Conflict, response.status);
            var role = response.role;
            Assert.Equal("scrub", role.RefName);
        }
        
        [Fact]
        public async void CreateOnNonExisting()
        {
            var roleDTO = new RoleDTO
            {
                RefName = "mastermind",
                GuildId = 1,
                RoleId = 6,
                Rank = 0
            };
            var response = await _roleRepository.Create(roleDTO);
            Assert.Equal(Status.Created, response.status);
            var role = response.role;
            Assert.Equal("mastermind", role.RefName);
        }

        [Fact]
        public async void GetHighestRankInGuildOnLegal()
        {
            var result1 = await _roleRepository.GetHighestRank(1);
            Assert.Equal(Status.Found, result1.status);
            Assert.Equal(2, result1.role.Rank);
            
            var result2 = await _roleRepository.GetHighestRank(2);
            Assert.Equal(Status.Found, result2.status);
            Assert.Equal(1, result2.role.Rank);
        }
        
        [Fact]
        public async void GetHighestRankInGuildOnIlllegal()
        {
            var result3 = await _roleRepository.GetHighestRank(3);
            Assert.Equal(Status.BadRequest, result3.status);
            Assert.Null(result3.role);
        }
        
        [Fact]
        public async void GetLowestRankInGuildOnLegal()
        {
            var result1 = await _roleRepository.GetLowestRank(1);
            Assert.Equal(Status.Found, result1.status);
            Assert.Equal(0, result1.role.Rank);
            
            var result2 = await _roleRepository.GetLowestRank(2);
            Assert.Equal(Status.Found, result2.status);
            Assert.Equal(0, result2.role.Rank);
        }
        
        [Fact]
        public async void GetLowestRankInGuildOnIllegal()
        {
            var result3 = await _roleRepository.GetLowestRank(3);
            Assert.Equal(Status.BadRequest, result3.status);
        }

        [Theory]
        [InlineData(-3, Status.Found, "scrub")]
        [InlineData(-103, Status.Found, "Intermediate")]
        [InlineData(-400, Status.Found, "pro")]
        public async Task GetRoleFromTime(int daysAgo, Status expectedStatus, string expectedName)
        {
            var joined = DateTime.Now.AddDays(daysAgo);
            var result = await _roleRepository.GetLevelRoleFromTime(joined, 1);
            Assert.Equal(expectedStatus, result.status);
            Assert.Equal(expectedName, result.role.RefName);
        }
        
        [Theory]
        [InlineData(3)]
        [InlineData(103)]
        [InlineData(400)]
        public async Task GetRoleFromIllegalTime(int daysAgo)
        {
            var joined = DateTime.Now.AddDays(daysAgo);
            var result = await _roleRepository.GetLevelRoleFromTime(joined, 1);
            Assert.Equal(Status.BadRequest, result.status);
            Assert.Null(result.role);
        }
        
        [Fact]
        //Find a way to test with discord members
        public async Task GetRoleFromMember()
        {
            
        }
        
        [Fact]
        //Find a way to test with discord members
        public async Task GetRoleFromIllegalMember()
        {
            
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        public async Task GetLevelNextWithLegalRank(int currRank, int nextRank)
        {
            var response = await _roleRepository.GetLevelNext(currRank, 1);
            Assert.Equal(Status.Found, response.status);
            Assert.Equal(nextRank, response.role.Rank);
        }
        
        [Theory]
        [InlineData(-2)]
        [InlineData(2)]
        [InlineData(100)]
        public async Task GetLevelNextWithIllegalRank(int currRank)
        {
            var response = await _roleRepository.GetLevelNext(currRank, 1);
            Assert.Equal(Status.BadRequest, response.status);
            Assert.Null(response.role);
        }

        [Theory]
        [InlineData(-1, 3)]
        [InlineData(0, 2)]
        [InlineData(1, 1)]
        [InlineData(2, 0)]
        [InlineData(50, 0)]
        public async Task GetAllLevelNextWithLegalRank(int currRank, int expectedSize)
        {
            var response = await _roleRepository.GetAllLevelNext(currRank, 1);
            Assert.Equal(expectedSize, response.Count());
        }
        
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(50, 3)]
        public async Task GetAllLevelPrevWithLegalRank(int currRank, int expectedSize)
        {
            var response = await _roleRepository.GetAllLevelPrev(currRank, 1);
            Assert.Equal(expectedSize, response.Count());
        }

        [Theory]
        [InlineData(-50, 49)]
        [InlineData(-100, 99)]
        public void GetDays(int days, int expectedDays)
        {
            var joined = DateTime.Now.AddDays(days);
            var calculatedDays = _roleRepository.GetDays(joined);
            Assert.Equal(Status.Found, calculatedDays.status);
            Assert.Equal(expectedDays, calculatedDays.days);
        }
        
        [Theory]
        [InlineData(100)]
        [InlineData(5)]
        public void GetDaysFromIllegalTime(int days)
        {
            var joined = DateTime.Now.AddDays(days);
            var calculatedDays = _roleRepository.GetDays(joined);
            Assert.Equal(Status.BadRequest, calculatedDays.status);
            Assert.Equal(0, calculatedDays.days);
        }
    }
}