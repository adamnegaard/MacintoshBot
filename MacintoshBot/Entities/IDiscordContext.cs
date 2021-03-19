using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MacintoshBot.Entities
{
    public interface IDiscordContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Image> Images { get; set; }
        DbSet<Group> Groups { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Role> LevelRoles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}