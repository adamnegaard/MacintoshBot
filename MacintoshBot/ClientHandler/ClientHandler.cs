using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using MacintoshBot.Commands;
using MacintoshBot.Models;
using MacintoshBot.Models.Channel;
using MacintoshBot.Models.Facts;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MacintoshBot.ClientHandler
{
    public class ClientHandler : IClientHandler
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IFactRepository _factRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _clientFactory;
        
        private readonly ILogger<ClientHandler> _logger;

        public ClientHandler(IUserRepository userRepository, IGroupRepository groupRepository,
            IMessageRepository messageRepository, ILevelRoleRepository levelRoleRepository,
            IChannelRepository channelRepository, IFactRepository factRepository, IHttpClientFactory clientFactory,
            ILogger<ClientHandler> logger)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _levelRoleRepository = levelRoleRepository;
            _channelRepository = channelRepository;
            _factRepository = factRepository;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task SelfAssignRoles(DiscordClient client, ulong guildId)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            if (server == null)
            {
                _logger.LogError("Could not find the server");
                return;
            }

            //Get the "roles" channel
            var roleChannel = await _channelRepository.Get("role", guildId);
            if (roleChannel.status != Status.Found)
            {
                _logger.LogError("Could not find the roles channel in the database");
                return;
            }

            var roleDiscordChannel =
                server.Channels.Values.FirstOrDefault(channel => channel.Id == roleChannel.channel.ChannelId);
            if (roleDiscordChannel == null)
            {
                _logger.LogError("Could not find the roles channel");
                return;
            }

            //Get the specific reaction message
            var message = await _messageRepository.GetMessageId("role", guildId);
            if (message.status != Status.Found)
            {
                _logger.LogError("Could not find the roles message in the database");
                return;
            }

            var assignMessage = await roleDiscordChannel.GetMessageAsync(message.messageId);
            if (assignMessage == null)
            {
                _logger.LogError("Could not find the roles message");
                return;
            }

            //assign the given reactions to the message
            foreach (var game in await _groupRepository.Get(guildId))
                await assignMessage.CreateReactionAsync(DiscordEmoji.FromName(client, game.EmojiName));
        }

        public async Task<DiscordEmbed> GetLevelEmbed(DiscordClient client, ulong guildId, string title, DiscordMember member)
        {
            //Start the embed with relevant fields
            var memberDays = _levelRoleRepository.GetDays(member.JoinedAt);
            
            //Get the user from the database
            var actualUser = await _userRepository.Get(member.Id, guildId);
            //If we can find the user in the database, return
            if (actualUser.status != Status.Found)
            {
                var errorMessage = $"Could not find user {member.DisplayName} in the database";
                _logger.LogError(errorMessage);
                return MacintoshEmbed.ErrorEmbed(errorMessage);
            }
            var levelEmbed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = $"Member for {memberDays.days} days",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = member.AvatarUrl
                }
            };
            //Add relevant fields
            levelEmbed.AddField("Level", actualUser.user.Level.ToString(), true);
            levelEmbed.AddField("Xp", actualUser.user.Xp.ToString(), true);
            var levelRole = await _levelRoleRepository.GetLevelFromDiscordMember(member, guildId);
            if (levelRole.status == Status.Found)
            {
                var discordRole = await DiscordRoleFromId(client, levelRole.role.RoleId, guildId);
                levelEmbed.AddField("Role", discordRole.Name, true);
            }

            return MacintoshEmbed.Create(levelEmbed);
        }

        public async Task<DiscordMessage> SendSelfAssignMessage(DiscordClient client, ulong guildId)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            if (server == null)
            {
                _logger.LogError("Could not find the server");
                return null;
            }

            //Get the "roles" channel
            var roleChannel = await _channelRepository.Get("role", guildId);
            if (roleChannel.status != Status.Found)
            {
                _logger.LogError("Could not find the roles channel in the database");
                return null;
            }

            var roleDiscordChannel =
                server.Channels.Values.FirstOrDefault(channel => channel.Id == roleChannel.channel.ChannelId);
            if (roleDiscordChannel == null)
            {
                _logger.LogError("Could not find the roles channel");
                return null;
            }

            //Get the reaction channel
            var messageBuilder = await GetReactionMessage(client, guildId);

            return await roleDiscordChannel.SendMessageAsync(messageBuilder);
        }
    
        //TODO Continue logging from here
        public async Task<DiscordRole> DiscordRoleFromId(DiscordClient client, ulong roleId, ulong guildId)
        {
            var server = client.Guilds.Values.FirstOrDefault(g => g.Id == guildId);
            if (server == null)
            {
                _logger.LogError("Could not find the server");
                return null;
            }

            var role = server.Roles.FirstOrDefault(k => k.Key == roleId).Value;
            if (role == null)
            {
                _logger.LogError("Could not find the specified role");
                return null;
            }

            return role;
        }
        
        public async Task<int> EvaluateUserLevelUpdrades(DiscordClient client)
        {
            var members = await _userRepository.Get();

            var upgrades = 0;
            foreach (var member in members)
            {
                if (member == null) continue;
                var guild = client.Guilds.FirstOrDefault(g => g.Key == member.GuildId).Value;
                var guildId = guild.Id;
                var discordMember = guild.Members.FirstOrDefault(m => m.Key == member.UserId).Value;
                if (discordMember.IsBot) continue;
                var memberDays = _levelRoleRepository.GetDays(discordMember.JoinedAt);
                    
                _logger.LogInformation($"User with name {discordMember.DisplayName} has been member for {memberDays.days} days");
                    
                var levelRole = await _levelRoleRepository.GetLevelFromDiscordMember(discordMember, guildId);
                if (levelRole.status != Status.Found) continue;
                    
                var nextMemberRole =
                    await _levelRoleRepository.GetLevelRoleFromTime(discordMember.JoinedAt, guildId);
                if (nextMemberRole.status != Status.Found) continue;

                //check if it's a new role (but since it might be an old role, we need to check if it's an upgrade and not a downgrade
                var upgradeFromCurrRole = await _levelRoleRepository.GetLevelNext(levelRole.role.Rank, guildId);
                if (upgradeFromCurrRole.status != Status.Found)
                {
                    continue;
                }
                //Do also a null check
                if (levelRole.role.RoleId != nextMemberRole.role.RoleId && upgradeFromCurrRole.role != null &&
                    nextMemberRole.role.RoleId == upgradeFromCurrRole.role.RoleId)
                {
                    var role = await DiscordRoleFromId(client, nextMemberRole.role.RoleId, guildId);
                    if (role == null) return 0;
                    await RevokeOtherRoles(client, discordMember, nextMemberRole.role, guildId);
                    await discordMember.GrantRoleAsync(role);
                    await NotifyMemberOfRoleUpgrade(client, discordMember, guild, role);
                    _logger.LogInformation( $"Succesfully upgraded user with name: {discordMember.DisplayName} from role: {levelRole.role.RefName} to {role.Name}");
                    upgrades++;
                }
            }
            _logger.LogInformation($"Successfully upgraded {upgrades} users roles");
            return upgrades;
        }

        public async Task DailyFact(DiscordClient client)
        {
            var guilds = client.Guilds.Values;

            foreach (var guild in guilds)
            {
                var factChannel = await _channelRepository.Get("dailyfacts", guild.Id);
                if (factChannel.status != Status.Found) continue;
                var factDiscordChannel =
                    guild.Channels.FirstOrDefault(c => c.Key == factChannel.channel.ChannelId).Value;
                if (factDiscordChannel == null) continue;
                await SendDailyFact(client, factDiscordChannel);
            }
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
            if (proRole.status != Status.Found) return;

            var discordRole = await DiscordRoleFromId(client, proRole.role.RoleId, guildId);

            //remove the old roles
            await RevokeOtherRoles(client, member, proRole.role, guildId);
            //Grant the new roles
            await member.GrantRoleAsync(discordRole);

            await member.GrantRoleAsync(modRole);
        }

        public async Task MakeUnMod(DiscordClient client, DiscordMember member, DiscordRole modRole, ulong guildId)
        {
            var levelRole = await _levelRoleRepository.GetLevelRoleFromTime(member.JoinedAt, guildId);
            if (levelRole.status != Status.Found) return;

            var discordRole = await DiscordRoleFromId(client, levelRole.role.RoleId, guildId);
            //remove the old roles
            await RevokeOtherRoles(client, member, levelRole.role, guildId);
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
                Title = $"Daily Fact #{fact.Id}",
                Description = fact.Text
            };
            var message = await channel.SendMessageAsync(MacintoshEmbed.Create(factEmbed));

            await message.CreateReactionAsync(DiscordEmoji.FromName(client, ":gift:"));
        }

        private async Task SendDailyFact(DiscordClient client, DiscordChannel channel)
        {
            try
            {
                var httpClient = _clientFactory.CreateClient("MacintoshBot");
                var jsonFactResponse = await httpClient.GetStringAsync("https://uselessfacts.jsph.pl/today.json?language=en");
                var factRead = JsonConvert.DeserializeObject<DailyFactJson>(jsonFactResponse);
                
                if (factRead == null) return;
                var (status, fact) = await _factRepository.Create(factRead.Text);
                
                if (status != Status.Created) return;
                await CreateFactMessage(client, fact, channel);
                _logger.LogInformation($"Successfully sent the daily fact #{fact.Id}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured when sending daily fact, error: {e}");
            }
        }

        private async Task NotifyMemberOfRoleUpgrade(DiscordClient client, DiscordMember member, DiscordGuild guild, DiscordRole role)
        {
            var levelEmbed =
                await GetLevelEmbed(client, guild.Id, $"Your role was upgraded to {role.Name} in {guild.Name}!", member);
            await member.SendMessageAsync(levelEmbed);
            _logger.LogInformation($"Succesfully informed {member.DisplayName} that their role was upgraded to ${role.Name}");
        }
    }
}