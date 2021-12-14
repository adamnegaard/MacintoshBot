using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MacintoshBot.Models;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using Microsoft.Extensions.Logging;

namespace MacintoshBot
{
    public partial class Bot
    {
        //A reaction is added to the self assign role message.
        private async Task OnReactionAdded(DiscordClient sender, MessageReactionAddEventArgs eventArgs)
        {
            _logger.LogInformation($"Recieved {nameof(OnReactionAdded)} event");
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
                _logger.LogInformation($"User with Id: {reactionUser.Id} reacted with emoji: {eventArgs.Emoji.GetDiscordName()}");
                var discordRole = await _clientHandler.DiscordRoleFromId(_client, role.roleId, guildId);
                await reactionUser.GrantRoleAsync(discordRole);
                _logger.LogInformation($"User with Id: {reactionUser.Id} was assigned role with Id: {discordRole.Id}");
            }
        }

        //A reaction is removed from the self assign role message
        private async Task OnReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs eventArgs)
        {
            _logger.LogInformation($"Recieved {nameof(OnReactionRemoved)} event");
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
                _logger.LogInformation($"User with Id: {reactionUser.Id} removed reaction with emoji: {eventArgs.Emoji.GetDiscordName()}");
                var discordRole = await _clientHandler.DiscordRoleFromId(_client, role.roleId, guildId);
                await reactionUser.RevokeRoleAsync(discordRole);
                _logger.LogInformation($"User with Id: {reactionUser.Id} was revoked role with Id: {discordRole.Id}");
            }
        }

        // When a member joins the group assign them their role on discord and in the database
        private async Task OnGuildMemberUpdated(DiscordClient client, GuildMemberUpdateEventArgs eventArgs)
        {
            _logger.LogInformation($"Recieved {nameof(OnGuildMemberUpdated)} event");
            var pendingAfter = eventArgs.PendingAfter;
            var roles = eventArgs.Member.Roles;

            var guildId = eventArgs.Guild.Id;
            var joinedMember = eventArgs.Member;
            
            // only assign the roles if user has passed screening and does not have any roles yet
            if (pendingAfter == false && !roles.Any())
            {
                _logger.LogInformation($"User with Id: {joinedMember.Id} joined guild with Id: {guildId}");
                await AssignNewUserRoles(joinedMember, guildId);
            } 
            else if (eventArgs.PendingBefore == true && pendingAfter == true)
            {
                _logger.LogInformation($"User with Id: {joinedMember.Id} was updated but has not accepted the screening");
            }
        }

        //Helper for creating a new user (OnMemberJoined).
        private async Task AssignNewUserRoles(DiscordMember member, ulong guildId)
        {
            //Create the user
            var createdUser = await _userRepository.Create(member.Id, guildId);
            if (createdUser.status != Status.Created)
            {
                _logger.LogError($"Received {createdUser.status.ToString()} when creating user with Id: {member.Id} in database");
                return;
            }

            //Get the scrub role
            var lowestRank = await _levelRoleRepository.GetLowestRank(guildId);
            if (lowestRank.status != Status.Found)
            {
                _logger.LogError("Could not find the lowest role in the database");
            }

            var discordRole = await _clientHandler.DiscordRoleFromId(_client, lowestRank.role.RoleId, guildId);
            await member.GrantRoleAsync(discordRole);
            _logger.LogInformation($"Assigned {discordRole.Name} to user with Id: {member.Id}");
        }

        //When a member leaves
        private async Task OnMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs eventArgs)
        {
            _logger.LogInformation($"Recieved {nameof(OnMemberRemoved)} event");
            var leftUser = eventArgs.Member;
            var guildId = eventArgs.Guild.Id;

            var status = await RemoveUser(leftUser, guildId);
            if (status != Status.Deleted)
            {
                _logger.LogInformation($"Could not delete user with Id: {leftUser.Id} from database");
                return;
            }

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
            
            _logger.LogInformation($"Sent member leave message: {message}");
        }

        private async Task OnGuildAvailable(DiscordClient client, GuildCreateEventArgs eventArgs)
        {
            _logger.LogInformation($"Recieved {nameof(OnGuildAvailable)} event");
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
                if (message.status != Status.Created)
                {
                    _logger.LogError($"Could not create assign message in database, got status {message.status.ToString()}");
                    return;
                }
                await _clientHandler.SelfAssignRoles(client, guildId);
                _logger.LogInformation($"Succesfully sent self assign message in guild with Id: {guildId}");
            }
            _logger.LogInformation($"Self assign message already existed, skipping...");
        }

        public async Task OnVoiceStateUpdate(DiscordClient client, VoiceStateUpdateEventArgs eventArgs)
        {
            _logger.LogInformation($"Recieved {nameof(OnVoiceStateUpdate)} event");
            var guildId = eventArgs.Guild.Id;
            //User enters a voice channel
            if ((eventArgs.Before == null || eventArgs.Before.Channel == null) && eventArgs.After != null)
            {
                _logger.LogInformation($"Member with Id: {eventArgs.User.Id} joined channel with Id {eventArgs.After.Channel.Id}");
                _xpGrantModel.EnterVoiceChannel(eventArgs.User.Id, guildId);
            }
            //User exits a voice channel
            else if (eventArgs.Before != null && (eventArgs.After == null || eventArgs.After.Channel == null))
            {
                var gainedXp = await _xpGrantModel.ExitVoiceChannel(eventArgs.User.Id, guildId);
                _logger.LogInformation($"Member with Id: {eventArgs.User.Id} left channel with Id {eventArgs.Before.Channel.Id}\nGained {gainedXp} XP");
            }
        }
        
        // When a role is updated
        public async Task OnGuildRoleUpdated(DiscordClient client, GuildRoleUpdateEventArgs eventArgs)
        {
            _logger.LogInformation($"Recieved {nameof(OnGuildRoleUpdated)} event");
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
                        _logger.LogInformation($"Successfully updated role with old Id: {eventArgs.RoleBefore.Id}");
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