// using System.Linq;
// using MacintoshBot.Entities;
// using MacintoshBot.Models;
// using MacintoshBot.Models.Image;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
// using Xunit;
//
// namespace MacintoshBot.Tests.Repositories
// {
//     public class ImageRepositoryTests
//     {
//         private readonly IImageRepository _imageRepository;
//         
//         public ImageRepositoryTests()
//         {
//             //Connection
//             var connection = new SqliteConnection("datasource=:memory:");
//             connection.Open();
//             
//             //Context
//             var builder = new DbContextOptionsBuilder<DiscordContext>().UseSqlite(connection);
//             var context = new DiscordTestContext(builder.Options);
//             context.Database.EnsureCreated();
//
//             _imageRepository = new ImageRepository(context, null);
//         }
//
//         [Fact]
//         public async void GetLocationOnExisting()
//         {
//             var response = await _imageRepository.GetLocation("poggers", 1);
//             Assert.Equal(Status.Found, response.status);
//             Assert.Equal("http://test/", response.location.OriginalString);
//         }
//         
//         [Fact]
//         public async void GetLocationOnNonExistingName()
//         {
//             var response = await _imageRepository.GetLocation("pawg", 1);
//             Assert.Equal(Status.BadRequest, response.status);
//         }
//         
//         [Fact]
//         public async void GetLocationOnNonExistingGuild()
//         {
//             var response = await _imageRepository.GetLocation("poggers", 2);
//             Assert.Equal(Status.BadRequest, response.status);
//         }
//
//         [Fact]
//         public async void CheckImageCountCorrect()
//         {
//             var images = await _imageRepository.Get(1);
//             Assert.Equal(2, images.Count());
//         }
//         
//         [Fact]
//         public async void CheckImageCountCorrectInNonExistingGuild()
//         {
//             var images = await _imageRepository.Get(2);
//             Assert.Equal(0, images.Count());
//         }
//     }
// }