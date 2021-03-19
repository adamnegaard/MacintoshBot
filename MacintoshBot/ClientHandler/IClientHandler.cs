using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using MacintoshBot.Entities;
using MacintoshBot.Models.Role;

namespace MacintoshBot
{
    public interface IClientHandler
    {
        Task SelfAssignRoles(DiscordClient client);
        Task<DiscordMessageBuilder> GetReactionMessage(DiscordClient client);
        Task<DiscordMessage> SendSelfAssignMessage(DiscordClient client);
        Task<int> EvaluateUserLevelUpdrades(DiscordClient client);
        Task MakeMemberMod(DiscordClient client, DiscordMember member, DiscordRole modRole);
        Task RevokeOtherRoles(DiscordMember member, RoleDTO newRole);
    }
}