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

namespace MacintoshBot.Models.File
{
    public class FileRepository : IFileRepository
    {
        private readonly IDiscordContext _context;

        public FileRepository(IDiscordContext context, IWebHostEnvironment env)
        {
            _context = context;
        }
        
        public async Task<(Status status, FileDTO file)> Get(string fileTitle, ulong guildId)
        {
            var file = await _context.Files.FirstOrDefaultAsync(i => i.Title.Equals(fileTitle) && i.GuildId == guildId);
            if (file == null)
            {
                return (Status.BadRequest, null);
            }
            return (Status.Found, new FileDTO
            {
                Title = file.Title,
                GuildId = file.GuildId,
                Location = file.Location
            });
        }

        public async Task<(Status status, FileDTO file)> Create(FileDTO file)
        {
            var existingFile = await Get(file.Title, file.GuildId);
            if (existingFile.status == Status.Found)
            {
                return (Status.Conflict, existingFile.file);
            }

            var fileCreate = new Entities.File
            {
                Title = file.Title,
                GuildId = file.GuildId,
                Location = file.Location,
            };

            var createdFile = await _context.Files.AddAsync(fileCreate);
            await _context.SaveChangesAsync();

            if (createdFile.Entity == null)
            {
                return (Status.Error, null);
            }

            return (Status.Created, new FileDTO
            {
                Title = createdFile.Entity.Title,
                GuildId = createdFile.Entity.GuildId,
                Location = createdFile.Entity.Location,
            });
        }

        public async Task<IEnumerable<string>> Get(ulong guildId)
        {
            return await _context.Files.Where(i => i.GuildId == guildId).Select(i => i.Title).ToListAsync();
        }
    }
}