using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using MacintoshBot.Models.Channel;
using MacintoshBot.Models.Facts;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;
using Newtonsoft.Json;

namespace MacintoshBot.ClientHandler
{
    public class ClientHandler : IClientHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IFactRepository _factRepository;

        public ClientHandler(IUserRepository userRepository, IGroupRepository groupRepository, IMessageRepository messageRepository, ILevelRoleRepository levelRoleRepository, IChannelRepository channelRepository, IFactRepository factRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _levelRoleRepository = levelRoleRepository;
            _channelRepository = channelRepository;
            _factRepository = factRepository;
        }
        
        public async Task SelfAssignRoles(DiscordClient client, ulong guildId)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            if (server == null)
            {
                await Console.Error.WriteLineAsync("Could not find the server");
                return; 
            }
            
            //Get the "roles" channel
            var roleChannelId = await _channelRepository.Get("role", guildId);
            var roleChannel = server.Channels.Values.FirstOrDefault(channel => channel.Id == roleChannelId);
            if (roleChannel == null)
            {
                await Console.Error.WriteLineAsync("Could not find the roles channel");
                return; 
            }
            
            //Get the specific reaction message
            var assignMessage = await roleChannel
                .GetMessageAsync(await _messageRepository.Get("role", guildId));
            if (assignMessage == null)
            {
                await Console.Error.WriteLineAsync("Could not find the roles message");
                return; 
            }

            //assign the given reactions to the message
            foreach (var game in await _groupRepository.Get(guildId))
            {
                await assignMessage.CreateReactionAsync(DiscordEmoji.FromName(client, game.EmojiName));
            }
        }
        
        public async Task<DiscordMessage> SendSelfAssignMessage(DiscordClient client, ulong guildId)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            if (server == null)
            {
                await Console.Error.WriteLineAsync("Could not find the server");
                return null;
            }
            
            //Get the "roles" channel
            var roleChannelId = await _channelRepository.Get("role", guildId);
            var roleChannel = server.Channels.Values.FirstOrDefault(channel => channel.Id == roleChannelId);
            if (roleChannel == null)
            {
                await Console.Error.WriteLineAsync("Could not find the roles channel");
                return null;
            }
            //Get the reaction channel
            var messageBuilder = await GetReactionMessage(client, guildId);

            return await roleChannel.SendMessageAsync(messageBuilder);
        }

        public async Task<DiscordRole> DiscordRoleFromId(DiscordClient client, ulong roleId, ulong guildId)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            if (server == null)
            {
                await Console.Error.WriteLineAsync("Could not find the server");
                return null;
            }

            var role = server.Roles.FirstOrDefault(k => k.Key == roleId).Value;
            if (role == null)
            {
                await Console.Error.WriteLineAsync("Could not find the specified role");
                return null;
            }

            return role;
        }
        
        //FIXME.
        public async Task<int> EvaluateUserLevelUpdrades(DiscordClient client)
        {
            var members = await _userRepository.GetAll();

            var upgrades = 0;
            foreach (var member in members)
            {
                if (member != null)
                {
                    var guild = client.Guilds.FirstOrDefault(g => g.Key == member.GuildId).Value;
                    var guildId = guild.Id;
                    var discordMember = guild.Members.FirstOrDefault(m => m.Key == member.UserId).Value;
                    
                    var levelRole = await _levelRoleRepository.GetLevelFromDiscordMember(discordMember, guildId);
                    var nextMemberRole = await _levelRoleRepository.GetLevelRoleFromTime(discordMember.JoinedAt, guildId);
                    
                    //check if it's a new role (but since it might be an old role, we need to check if it's an upgrade and not a downgrade
                    var upgradeFromCurrRole = await _levelRoleRepository.GetLevelNext(levelRole.Rank, guildId);
                    //Do also a null check
                    if (levelRole.RoleId != nextMemberRole.RoleId && upgradeFromCurrRole != null && nextMemberRole.RoleId == upgradeFromCurrRole.RoleId)
                    {
                        var role = await DiscordRoleFromId(client, nextMemberRole.RoleId, guildId);
                        if (role == null)
                        {
                            return 0;
                        }
                        await RevokeOtherRoles(client, discordMember, nextMemberRole, guildId);
                        await discordMember.GrantRoleAsync(role);
                        upgrades++;
                    }
                }
            }

            return upgrades;
        }

        public async Task DailyFact(DiscordClient client)
        {
            var guilds = client.Guilds.Values;

            foreach (var guild in guilds)
            {
                var factChannelId = await _channelRepository.Get("dailyfacts", guild.Id);
                var factChannel = guild.Channels.FirstOrDefault(c => c.Key == factChannelId).Value;
                if (factChannel == null)
                {
                    continue;
                }
                await SendDailyFact(client, factChannel);
            }
        }

        private async Task SendDailyFact(DiscordClient client, DiscordChannel channel)
        {
            
            var jsonFact = new WebClient().DownloadString("https://uselessfacts.jsph.pl/today.json?language=en");
            var factText = JsonConvert.DeserializeObject<DailyFactJson>(jsonFact);

            var factCreate = new FactDTO
            {
                Text = factText?.Text
            };
            var fact = await _factRepository.Create(factCreate);

            await CreateFactMessage(client, fact, channel);
        }

        public async Task<DiscordMessageBuilder> GetReactionMessage(DiscordClient client, ulong guildId)
        {
            var messageBuilder = new DiscordMessageBuilder();
            messageBuilder.Content += "**React to give yourself one or more server roles:**";
            foreach (var game in await _groupRepository.Get(guildId))
            {
                messageBuilder.Content += $"\n\n{DiscordEmoji.FromName(client, game.EmojiName)}: {game.FullName}";
                if (game.IsGame) messageBuilder.Content += " Gamer";
            }
            return messageBuilder;
        }

        //Make a member mod.
        public async Task MakeMemberMod(DiscordClient client, DiscordMember member, DiscordRole modRole, ulong guildId)
        {
            var proRole = await _levelRoleRepository.GetHighestRank(guildId);
            if (proRole == null)
            {
                return;
                
            }

            var discordRole = await DiscordRoleFromId(client, proRole.RoleId, guildId);
            
            //remove the old roles
            await RevokeOtherRoles(client, member, proRole, guildId);
            //Grant the new roles
            await member.GrantRoleAsync(discordRole);

            await member.GrantRoleAsync(modRole);
        }

        public async Task MakeUnMod(DiscordClient client, DiscordMember member, DiscordRole modRole, ulong guildId)
        {
            var levelRole = await _levelRoleRepository.GetLevelRoleFromTime(member.JoinedAt, guildId);
            if (levelRole == null)
            {
                return;
            }
            
            var discordRole = await DiscordRoleFromId(client, levelRole.RoleId, guildId);
            //remove the old roles
            await RevokeOtherRoles(client, member, levelRole, guildId);
            //Grant the new roles
            await member.GrantRoleAsync(discordRole);
            //Revoke the modeator role
            await member.RevokeRoleAsync(modRole);
        }

        public async Task RevokeOtherRoles(DiscordClient client, DiscordMember member, RoleDTO newRole, ulong guildId)
        {
            var rolesBelow = await _levelRoleRepository.GetAllLevelPrev(newRole.Rank, guildId);
            var rolesAbove = await _levelRoleRepository.GetAllLevelNext(newRole.Rank, guildId);
            //revoke the other roles
            foreach (var role in rolesAbove.Concat(rolesBelow))
            {
                var discordRole = await DiscordRoleFromId(client, role.RoleId, guildId);
                await member.RevokeRoleAsync(discordRole);
            }
        }

        public async Task CreateFactMessage(DiscordClient client, FactDTO fact, DiscordChannel channel)
        {
            var factEmbed = new DiscordEmbedBuilder
            {
                Title = $"Daily Fact #{fact.Id}!",
                Description = fact.Text,
                Timestamp = DateTimeOffset.Now
            };
            var message = await channel.SendMessageAsync(embed: factEmbed);
            
            await message.CreateReactionAsync(DiscordEmoji.FromName(client, ":gift:"));
        }
    }
}