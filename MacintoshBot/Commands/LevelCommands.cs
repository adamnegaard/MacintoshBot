using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.ClientHandler;
using MacintoshBot.Models;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;

namespace MacintoshBot.Commands
{
    //[Group("Level")]
    [Description("Commands related to your server level")]
    public class LevelCommands : BaseCommandModule
    {
        private readonly IClientHandler _clientHandler;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IUserRepository _userRepository;

        public LevelCommands(IUserRepository userRepository, ILevelRoleRepository levelRoleRepository,
            IClientHandler clientHandler)
        {
            _userRepository = userRepository;
            _levelRoleRepository = levelRoleRepository;
            _clientHandler = clientHandler;
        }

        [Command("level")]
        [Description("See your current level and how many days you have been a member of the server")]
        public async Task Level(CommandContext ctx, [Description("Member to see level of (empty if yourself)")]
            DiscordMember member = null)
        {
            var guildId = ctx.Guild.Id;
            //Check if the member is null, if it is set the member to the one who queried.
            if (member == null) member = ctx.Member;
            //Get the user from the database
            var actualUser = await _userRepository.Get(member.Id, guildId);
            //If we can find the user in the database, return
            if (actualUser.status != Status.Found)
            {
                await ctx.Channel.SendMessageAsync($"Could not find user {member.DisplayName} in the database");
                return;
            }

            var timeResponse = _levelRoleRepository.GetDays(member.JoinedAt);
            //Start the embed with relevant fields
            var levelEmbed = new DiscordEmbedBuilder
            {
                Title = $"{member.DisplayName}'s Profile",
                Description = $"Member for {timeResponse.days} days",
                ImageUrl = member.AvatarUrl,
                Color = DiscordColor.Aquamarine
            };
            //Add relevant fields
            levelEmbed.AddField("Level", actualUser.user.Level.ToString());
            levelEmbed.AddField("Xp", actualUser.user.Xp.ToString());
            var levelRole = await _levelRoleRepository.GetLevelFromDiscordMember(member, guildId);
            if (levelRole.status != Status.Found)
            {
                var discordRole = await _clientHandler.DiscordRoleFromId(ctx.Client, levelRole.role.RoleId, guildId);
                levelEmbed.AddField("Role", discordRole.Name);
            }

            //Send the embed to the channel.
            await ctx.Channel.SendMessageAsync(levelEmbed);
        }
    }
}