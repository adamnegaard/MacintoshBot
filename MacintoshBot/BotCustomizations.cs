using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MacintoshBot.Entities;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.ServerConstants;


namespace MacintoshBot
{
    public partial class Bot
    {
        //A reaction is added to the self assign role message.
        private async Task OnReactionAdded(DiscordClient sender, MessageReactionAddEventArgs eventArgs)
        {
            //Get the needed variables
            var messageId = eventArgs.Message.Id;
            var discordRole = await _groupRepository.GetFromEmoji(eventArgs.Emoji.GetDiscordName());
            var reactionUser = eventArgs.User as DiscordMember;
            
            if(reactionUser == null || reactionUser.IsBot)
            {
                return;
            }
            //Perform the role update
            if (messageId == await _messageRepository.Get("role"))
            {
                await reactionUser.GrantRoleAsync(discordRole).ConfigureAwait(false);
            }
        }
        
        //A reaction is removed from the self assign role message
        private async Task OnReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs eventArgs)
        {
            //Get the needed variables
            var messageId = eventArgs.Message.Id;
            var discordRole = await _groupRepository.GetFromEmoji(eventArgs.Emoji.GetDiscordName());
            var reactionUser = eventArgs.User as DiscordMember;
            
            if(reactionUser == null || reactionUser.IsBot)
            {
                return;
            }
            //Perform the role update
            if (messageId == await _messageRepository.Get("role"))
            {
                await reactionUser.RevokeRoleAsync(discordRole).ConfigureAwait(false);
            }
        }

        //When a member joins the group assign them their role on discord and in the database
        private async Task OnMemberJoined(DiscordClient client, GuildMemberAddEventArgs eventArgs)
        {
            var joinedMember = eventArgs.Member;
            await AssignNewUserRoles(joinedMember);
        }
        
        //Helper for creating a new user (OnMemberJoined).
        private async Task AssignNewUserRoles(DiscordMember member)
        {
            //Create the user
            await _userRepository.Create(member.Id);
            //Get the scrub role
            var lowestRank = await _levelRoleRepository.GetLowestRank();
            if (lowestRank == null)
            {
                return;
            }
            await member.GrantRoleAsync(lowestRank.DiscordRole).ConfigureAwait(false);
        }
        
        //When a member leaves
        private async Task OnMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs eventArgs)
        {
            var leftUser = eventArgs.Member;
            await NotifyOfMemberLeave(leftUser);
            await RemoveUser(leftUser);
        }

        private async Task NotifyOfMemberLeave(DiscordMember member)
        {
            var message = RandomMemberLeaveMessage(member.DisplayName);
            var server = _client.Guilds.Values.FirstOrDefault(g => g.Id == (ulong) Guild.Main);
            var welcomeChannel = server?.Channels.Values.FirstOrDefault(c => c.Id == (ulong) Channel.NewMembers);
            await welcomeChannel.SendMessageAsync(message); 
        }
        
        private async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs eventArgs)
        {
            var assignMessage = await _messageRepository.Get("role");
            if (assignMessage == 0)
            {
                var newAssignMessage = await _clientHandler.SendSelfAssignMessage(client);
                var roleMessage = new MessageDTO
                {
                    DiscordId = newAssignMessage.Id,
                    RefName = "role"
                };
                await _messageRepository.Create(roleMessage);
                await _clientHandler.SelfAssignRoles(client);
            }
            //Start the user evaluation cycle
            //await StartUserEvaluation(client).ConfigureAwait(false);
        }

        public async Task OnVoiceStateUpdate(DiscordClient client, VoiceStateUpdateEventArgs eventArgs)
        {
            if (eventArgs.Before.Channel == null && eventArgs.After.Channel != null)
            {
                _xpGrantModel.EnterVoiceChannel(eventArgs.User.Id);
            }
            else if (eventArgs.Before.Channel != null && eventArgs.After.Channel == null)
            {
                var gainedXp = await _xpGrantModel.ExitVoiceChannel(eventArgs.User.Id);
            }
            
        }

        //Helper for removing a user
        private async Task RemoveUser(DiscordMember member)
        {
            await _userRepository.Delete(member.Id);
        }

        private string RandomMemberLeaveMessage(string memberName)
        {
            var random = new Random();
            var value = random.Next(3);
            switch (value)
            {
                case 0:
                    return $"Oh no, we have a leaver, say goodbye to {memberName}";
                case 1:
                    return $"{memberName} slid out of the chat...";
                case 2:
                    return $"Bye bye {memberName}!";
                case 3:
                    return $"Now that {memberName} left, we have room for a new member!"; 
                default:
                    return $"Thanks for the fun times {memberName}"; 
            }
        }
    }
}