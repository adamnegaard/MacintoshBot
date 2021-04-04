using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Image
{
    public class ImageRepository : IImageRepository
    {
        private readonly IDiscordContext _context;

        public ImageRepository(IDiscordContext context, IWebHostEnvironment env)
        {
            _context = context;
        }
        
        public async Task<(Status status, ImageDTO image)> Get(string imageTitle, ulong guildId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(i => i.Title.Equals(imageTitle) && i.GuildId == guildId);
            if (image == null)
            {
                return (Status.BadRequest, null);
            }
            return (Status.Found, new ImageDTO
            {
                Title = image.Title,
                GuildId = image.GuildId,
                Location = image.Location
            });
        }

        public async Task<(Status status, ImageDTO image)> Create(ImageDTO image)
        {
            var existingImage = await Get(image.Location, image.GuildId);
            if (existingImage.status == Status.Found)
            {
                return (Status.Conflict, existingImage.image);
            }

            var imageCreate = new Entities.Image
            {
                Title = image.Title,
                GuildId = image.GuildId,
                Location = image.Location,
            };

            var createdImage = await _context.Images.AddAsync(imageCreate);
            await _context.SaveChangesAsync();

            if (createdImage.Entity == null)
            {
                return (Status.Error, null);
            }

            return (Status.Created, new ImageDTO
            {
                Title = createdImage.Entity.Title,
                GuildId = createdImage.Entity.GuildId,
                Location = createdImage.Entity.Location,
            });
        }

        public async Task<IEnumerable<string>> Get(ulong guildId)
        {
            return await _context.Images.Where(i => i.GuildId == guildId).Select(i => i.Title).ToListAsync();
        }
    }
}