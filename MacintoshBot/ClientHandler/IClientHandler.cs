using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using MacintoshBot.Models.Facts;
using MacintoshBot.Models.Role;

namespace MacintoshBot.ClientHandler
{
    public interface IClientHandler
    {
        Task SelfAssignRoles(DiscordClient client, ulong guildId);
        Task<DiscordMessageBuilder> GetReactionMessage(DiscordClient client, ulong guildId);
        Task<DiscordMessage> SendSelfAssignMessage(DiscordClient client, ulong guildId);
        Task<DiscordRole> DiscordRoleFromId(DiscordClient client, ulong roleId, ulong guildId);
        Task<int> EvaluateUserLevelUpdrades(DiscordClient client);
        Task DailyFact(DiscordClient client);
        Task MakeMemberMod(DiscordClient client, DiscordMember member, DiscordRole modRole, ulong guildId);
        Task MakeUnMod(DiscordClient client, DiscordMember member, DiscordRole modRole, ulong guildId);
        Task RevokeOtherRoles(DiscordClient client, DiscordMember member, RoleDTO newRole, ulong guildId);
        Task CreateFactMessage(DiscordClient client, FactDTO fact, DiscordChannel channel);
    }
}