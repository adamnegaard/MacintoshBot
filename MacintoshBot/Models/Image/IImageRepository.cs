using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MacintoshBot.Models.Image
{
    public interface IImageRepository
    {
        public Task<(Status status, Uri location)> GetLocation(string imageTitle, ulong guildId);

        public Task<IEnumerable<string>> Get(ulong guildId);
    }
}