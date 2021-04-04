using System.Collections.Generic;
using System.Threading.Tasks;

namespace MacintoshBot.Models.File
{
    public interface IFileRepository
    {
        public Task<(Status status, FileDTO file)> Get(string fileTitle, ulong guildId);
        public Task<(Status status, FileDTO file)> Create(FileDTO file);
        public Task<IEnumerable<string>> Get(ulong guildId);
    }
}