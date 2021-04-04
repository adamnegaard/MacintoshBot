using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MacintoshBot.Models.Image
{
    public interface IImageRepository
    {
        public Task<(Status status, ImageDTO image)> Get(string imageTitle, ulong guildId);
        public Task<(Status status, ImageDTO image)> Create(ImageDTO image);
        public Task<IEnumerable<string>> Get(ulong guildId);
    }
}