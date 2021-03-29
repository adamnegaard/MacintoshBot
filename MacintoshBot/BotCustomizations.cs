using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MacintoshBot.Models.Message;
using MacintoshBot.ServerConstants;


namespace MacintoshBot
{
    public partial class Bot
    {
        //A reaction is added to the self assign role message.
        private async Task OnReactionAdded(DiscordClient sender, MessageReactionAddEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            //Get the needed variables
            var messageId = eventArgs.Message.Id;
            var roleId = await _groupRepository.GetFromEmoji(eventArgs.Emoji.GetDiscordName(), guildId);
            var reactionUser = eventArgs.User as DiscordMember;
            
            if(reactionUser == null || reactionUser.IsBot)
            {
                return;
            }
            //Perform the role update
            if (messageId == await _messageRepository.Get("role", guildId))
            {
                var discordRole = await _clientHandler.DiscordRoleFromId(_client, roleId, guildId);
                await reactionUser.GrantRoleAsync(discordRole).ConfigureAwait(false);
            }
        }
        
        //A reaction is removed from the self assign role message
        private async Task OnReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            //Get the needed variables
            var messageId = eventArgs.Message.Id;
            var roleId = await _groupRepository.GetFromEmoji(eventArgs.Emoji.GetDiscordName(), guildId);
            var reactionUser = eventArgs.User as DiscordMember;
            
            if(reactionUser == null || reactionUser.IsBot)
            {
                return;
            }
            //Perform the role update
            if (messageId == await _messageRepository.Get("role", guildId))
            {
                var discordRole = await _clientHandler.DiscordRoleFromId(_client, roleId, guildId);
                await reactionUser.RevokeRoleAsync(discordRole).ConfigureAwait(false);
            }
        }

        //When a member joins the group assign them their role on discord and in the database
        private async Task OnGuildMemberUpdated(DiscordClient client, GuildMemberUpdateEventArgs eventArgs)
        {
            var pendingAfter = eventArgs.PendingAfter;
            if (pendingAfter == false)
            {
                var guildId = eventArgs.Guild.Id;
                var joinedMember = eventArgs.Member;
                await AssignNewUserRoles(joinedMember, guildId);
            }
        }
        
        //Helper for creating a new user (OnMemberJoined).
        private async Task AssignNewUserRoles(DiscordMember member, ulong guildId)
        {
            //Create the user
            await _userRepository.Create(member.Id, guildId);
            //Get the scrub role
            var lowestRank = await _levelRoleRepository.GetLowestRank(guildId);
            if (lowestRank == null)
            {
                return;
            }

            var discordRole = await _clientHandler.DiscordRoleFromId(_client, lowestRank.DiscordRoleId, guildId);
            await member.GrantRoleAsync(discordRole);
        }
        
        //When a member leaves
        private async Task OnMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs eventArgs)
        {
            var leftUser = eventArgs.Member;
            var guildId = eventArgs.Guild.Id;
            await NotifyOfMemberLeave(leftUser, guildId);
            await RemoveUser(leftUser, guildId);
        }

        private async Task NotifyOfMemberLeave(DiscordMember member, ulong guildId)
        {
            var message = RandomMemberLeaveMessage(member.DisplayName);
            var server = _client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            var welcomeChannel = server?.Channels.Values.FirstOrDefault(c => c.Id == (ulong) Channel.NewMembers);
            await welcomeChannel.SendMessageAsync(message); 
        }
        
        private async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            var assignMessage = await _messageRepository.Get("role", guildId);
            if (assignMessage == 0)
            {
                var newAssignMessage = await _clientHandler.SendSelfAssignMessage(client, guildId);
                var roleMessage = new MessageDTO
                {
                    DiscordId = newAssignMessage.Id,
                    GuildId = guildId,
                    RefName = "role"
                };
                await _messageRepository.Create(roleMessage);
                await _clientHandler.SelfAssignRoles(client, guildId);
            }
        }

        public async Task OnVoiceStateUpdate(DiscordClient client, VoiceStateUpdateEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            if (eventArgs.Before.Channel == null && eventArgs.After.Channel != null)
            {
                _xpGrantModel.EnterVoiceChannel(eventArgs.User.Id, guildId);
            }
            else if (eventArgs.Before.Channel != null && eventArgs.After.Channel == null)
            {
                var gainedXp = await _xpGrantModel.ExitVoiceChannel(eventArgs.User.Id, guildId);
            }
            
        }

        //Helper for removing a user
        private async Task RemoveUser(DiscordMember member, ulong guildId)
        {
            await _userRepository.Delete(member.Id, guildId);
        }

        private string RandomMemberLeaveMessage(string memberName)
        {
            var random = new Random();
            var value = random.Next(2);
            switch (value)
            {
                case 0:
                    return $"Oh no, we have a leaver, say goodbye to {memberName}";
                case 1:
                    return $"{memberName} slid out of the chat...";
                case 2:
                    return $"Bye bye {memberName}!";
                default:
                    return $"Thanks for the fun times {memberName}"; 
            }
        }
    }
}