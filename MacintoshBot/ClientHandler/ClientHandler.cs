using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using MacintoshBot.Entities;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;
using MacintoshBot.ServerConstants;

namespace MacintoshBot
{
    public class ClientHandler : IClientHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILevelRoleRepository _levelRoleRepository;

        public ClientHandler(IUserRepository userRepository, IGroupRepository groupRepository, IMessageRepository messageRepository, ILevelRoleRepository levelRoleRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _levelRoleRepository = levelRoleRepository;
        }
        
        public async Task SelfAssignRoles(DiscordClient client)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == (ulong) Guild.Main);
            if (server == null)
            {
                await Console.Error.WriteLineAsync("Could not find the server");
                return; 
            }
            
            //Get the "roles" channel
            var roleChannel = server.Channels.Values.FirstOrDefault(channel => channel.Id == (ulong) Channel.RoleChannel);
            if (roleChannel == null)
            {
                await Console.Error.WriteLineAsync("Could not find the roles channel");
                return; 
            }
            
            //Get the specific reaction message
            var assignMessage = await roleChannel
                .GetMessageAsync(await _messageRepository.Get("role"));
            if (assignMessage == null)
            {
                await Console.Error.WriteLineAsync("Could not find the roles message");
                return; 
            }

            //assign the given reactions to the message
            foreach (var game in await _groupRepository.Get())
            {
                await assignMessage.CreateReactionAsync(DiscordEmoji.FromName(client, game.EmojiName));
            }
        }
        
        public async Task<DiscordMessage> SendSelfAssignMessage(DiscordClient client)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == (ulong) Guild.Main);
            if (server == null)
            {
                await Console.Error.WriteLineAsync("Could not find the server");
                return null;
            }
            
            //Get the "roles" channel
            var roleChannel = server.Channels.Values.FirstOrDefault(channel => channel.Id == (ulong) Channel.RoleChannel);
            if (roleChannel == null)
            {
                await Console.Error.WriteLineAsync("Could not find the roles channel");
                return null;
            }
            //Get the reaction channel
            var messageBuilder = await GetReactionMessage(client);

            return await roleChannel.SendMessageAsync(messageBuilder);
        }

        public async Task<int> EvaluateUserLevelUpdrades(DiscordClient client)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == (ulong) Guild.Main);
            if (server == null)
            {
                return 0 ;
            }

            var upgrades = 0;
            Console.WriteLine("ran updates");
            foreach (var member in server.Members.Values)
            {
                if (member != null && !member.IsBot)
                {
                    var userMember = await _userRepository.Get(member.Id);
                    if (userMember == null) continue;
            
            
                    var levelRole = await _levelRoleRepository.GetLevelFromDiscordMember(member);
                    var nextMemberRole = await _levelRoleRepository.GetLevelRoleFromTime(member.JoinedAt);
                    
                    //check if it's a new role (but since it might be an old role, we need to check if it's an upgrade and not a downgrade
                    var upgradeFromCurrRole = await _levelRoleRepository.GetLevelNext(levelRole.Rank);
                    //Do also a null check
                    if (levelRole.DiscordRoleId != nextMemberRole.DiscordRoleId && upgradeFromCurrRole != null && nextMemberRole.DiscordRoleId == upgradeFromCurrRole.DiscordRoleId)
                    {
                        await RevokeOtherRoles(member, nextMemberRole);
                        await member.GrantRoleAsync(nextMemberRole.DiscordRole);
                        upgrades++;
                    }
                }
            }

            return upgrades;
        }

        public async Task<DiscordMessageBuilder> GetReactionMessage(DiscordClient client)
        {
            var messageBuilder = new DiscordMessageBuilder();
            messageBuilder.Content += "**React to give yourself one or more server roles:**";
            foreach (var game in await _groupRepository.Get())
            {
                messageBuilder.Content += $"\n\n{DiscordEmoji.FromName(client, game.EmojiName)}: {game.FullName}";
                if (game.IsGame) messageBuilder.Content += " Gamer";
            }
            return messageBuilder;
        }

        //Make a member mod.
        public async Task MakeMemberMod(DiscordClient client, DiscordMember member, DiscordRole modRole)
        {
            var proRole = await _levelRoleRepository.GetHighestRank();
            if (proRole == null)
            {
                return;
                
            }
            //remove the old roles
            await RevokeOtherRoles(member, proRole);
            //Grant the new roles
            await member.GrantRoleAsync(proRole.DiscordRole);

            await member.GrantRoleAsync(modRole);
        }

        public async Task RevokeOtherRoles(DiscordMember member, RoleDTO newRole)
        {
            var rolesBelow = await _levelRoleRepository.GetAllLevelPrev(newRole.Rank);
            var rolesAbove = await _levelRoleRepository.GetAllLevelNext(newRole.Rank);
            //revoke the other roles
            foreach (var role in rolesAbove.Concat(rolesBelow))
            {
                await member.RevokeRoleAsync(role.DiscordRole);
            }
        }
    }
}