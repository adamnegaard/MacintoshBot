using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Entities
{
    public interface IDiscordContext
    {
        DbSet<User> Members { get; set; }
        DbSet<File> Files { get; set; }
        DbSet<Group> Groups { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Role> LevelRoles { get; set; }
        DbSet<Channel> Channels { get; set; }
        DbSet<Fact> Facts { get; set; }
        DbSet<VoiceState> VoiceStates { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}