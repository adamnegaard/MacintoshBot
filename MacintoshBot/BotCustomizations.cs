using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MacintoshBot.Models;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;

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
            var role = await _groupRepository.GetRoleIdFromEmoji(eventArgs.Emoji.GetDiscordName(), guildId);
            if (role.status != Status.Found) return;
            var reactionUser = eventArgs.User as DiscordMember;

            if (reactionUser == null || reactionUser.IsBot) return;
            //Perform the role update
            var message = await _messageRepository.GetMessageId("role", guildId);
            if (message.status == Status.Found && messageId == message.messageId)
            {
                var discordRole = await _clientHandler.DiscordRoleFromId(_client, role.roleId, guildId);
                await reactionUser.GrantRoleAsync(discordRole).ConfigureAwait(false);
            }
        }

        //A reaction is removed from the self assign role message
        private async Task OnReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            //Get the needed variables
            var messageId = eventArgs.Message.Id;
            var role = await _groupRepository.GetRoleIdFromEmoji(eventArgs.Emoji.GetDiscordName(), guildId);
            if (role.status != Status.Found) return;
            var reactionUser = eventArgs.User as DiscordMember;

            if (reactionUser == null || reactionUser.IsBot) return;
            //Perform the role update
            var message = await _messageRepository.GetMessageId("role", guildId);
            if (message.status == Status.Found && messageId == message.messageId)
            {
                var discordRole = await _clientHandler.DiscordRoleFromId(_client, role.roleId, guildId);
                await reactionUser.RevokeRoleAsync(discordRole).ConfigureAwait(false);
            }
        }

        //When a member joins the group assign them their role on discord and in the database
        private async Task OnGuildMemberUpdated(DiscordClient client, GuildMemberUpdateEventArgs eventArgs)
        {
            var pendingAfter = eventArgs.PendingAfter;
            var roles = eventArgs.Member.Roles;
            if (pendingAfter == false && !roles.Any())
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
            var createdUser = await _userRepository.Create(member.Id, guildId);
            if (createdUser.status != Status.Created) return;

            //Get the scrub role
            var lowestRank = await _levelRoleRepository.GetLowestRank(guildId);
            if (lowestRank.status != Status.Found) return;

            var discordRole = await _clientHandler.DiscordRoleFromId(_client, lowestRank.role.RoleId, guildId);
            await member.GrantRoleAsync(discordRole);
        }

        //When a member leaves
        private async Task OnMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs eventArgs)
        {
            var leftUser = eventArgs.Member;
            var guildId = eventArgs.Guild.Id;

            var status = await RemoveUser(leftUser, guildId);
            if (status != Status.Deleted) return;

            await NotifyOfMemberLeave(leftUser, guildId);
        }

        private async Task NotifyOfMemberLeave(DiscordMember member, ulong guildId)
        {
            var message = RandomMemberLeaveMessage(member.DisplayName);
            var server = _client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            if (server == null) return;
            var newMemberChannel = await _channelRepository.Get("newmembers", guildId);
            if (newMemberChannel.status != Status.Found) return;
            var newMemberDiscordChannel =
                server.Channels.Values.FirstOrDefault(c => c.Id == newMemberChannel.channel.ChannelId);
            if (newMemberDiscordChannel == null) return;
            await newMemberDiscordChannel.SendMessageAsync(message);
        }

        // private async Task CreateApplicationCommands(DiscordClient client)
        // {
        //     
        // }

        private async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            var assignMessage = await _messageRepository.GetMessageId("role", guildId);
            if (assignMessage.status != Status.Found)
            {
                var newAssignMessage = await _clientHandler.SendSelfAssignMessage(client, guildId);
                var roleMessage = new MessageDTO
                {
                    MessageId = newAssignMessage.Id,
                    GuildId = guildId,
                    RefName = "role"
                };
                var message = await _messageRepository.Create(roleMessage);
                if (message.status != Status.Created) return;
                await _clientHandler.SelfAssignRoles(client, guildId);
            }
        }

        public async Task OnVoiceStateUpdate(DiscordClient client, VoiceStateUpdateEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            //User enters a voice channel
            if ((eventArgs.Before == null || eventArgs.Before.Channel == null) && eventArgs.After != null)
            {
                _xpGrantModel.EnterVoiceChannel(eventArgs.User.Id, guildId);
            }
            //User exits a voice channel
            else if (eventArgs.Before != null && (eventArgs.After == null || eventArgs.After.Channel == null))
            {
                var gainedXp = await _xpGrantModel.ExitVoiceChannel(eventArgs.User.Id, guildId);
            }
        }
        
        public async Task OnGuildRoleUpdated(DiscordClient client, GuildRoleUpdateEventArgs eventArgs)
        {
            var guildId = eventArgs.Guild.Id;
            if (eventArgs.RoleBefore != null)
            {
                var (status, role) = await _levelRoleRepository.Get(eventArgs.RoleBefore.Id, guildId);
                if (status == Status.Found)
                {
                    if (eventArgs.RoleAfter != null)
                    {
                        // TODO: Be able to modify the name
                        var roleUpdate = new RoleUpdateDTO
                        {
                            Rank = role.Rank,
                            RoleId = eventArgs.RoleAfter.Id
                        };
                        await _levelRoleRepository.Update(roleUpdate, eventArgs.RoleBefore.Id, guildId);
                    }
                }
            }
        }

        //Helper for removing a user
        private async Task<Status> RemoveUser(DiscordMember member, ulong guildId)
        {
            return await _userRepository.Delete(member.Id, guildId);
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