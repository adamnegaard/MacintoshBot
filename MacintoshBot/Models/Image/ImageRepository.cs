using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Models.Image
{
    public class ImageRepository : IImageRepository
    {
        private readonly IDiscordContext _context;

        public ImageRepository(IDiscordContext context)
        {
            _context = context;
        }
        
        public async Task<Uri> Get(string imageTitle)
        {
            var image = await _context.Images.FirstOrDefaultAsync(i => i.Title == imageTitle);
            if (image == null)
            {
                return null;
            }
            return image.Location;
        }

        public async Task<IEnumerable<string>> Get()
        {
            return await _context.Images.Select(i => i.Title).ToListAsync();
        }
    }
}