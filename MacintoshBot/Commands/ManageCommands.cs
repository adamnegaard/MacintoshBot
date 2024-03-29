﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.ClientHandler;
using MacintoshBot.Models;
using MacintoshBot.Models.Channel;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;

namespace MacintoshBot.Commands
{
    [Description("Roles related to management (Only for admins and moderators)")]
    [RequirePermissions(Permissions.ManageRoles)]
    public class ManageCommands : BaseCommandModule
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IClientHandler _clientHandler;
        private readonly IGroupRepository _groupRepository;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IMessageRepository _messageRepository;

        public ManageCommands(IClientHandler clientHandler, IGroupRepository groupRepository,
            IMessageRepository messageRepository, ILevelRoleRepository levelRoleRepository,
            IChannelRepository channelRepository)
        {
            _clientHandler = clientHandler;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _levelRoleRepository = levelRoleRepository;
            _channelRepository = channelRepository;
        }

        [Command(nameof(MakeMod))]
        [Description("Make a member moderator")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task MakeMod(CommandContext ctx, [Description("Discord member to make moderator")]
            DiscordMember member)
        {
            var guildId = ctx.Guild.Id;
            if (member == null)
            {
                await ctx.RespondAsync("No member specified for making moderator");
                return;
            }

            var modRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name.ToLower().Contains("mod"));
            if (modRole == null)
            {
                await ctx.RespondAsync("Internal error, could not find moderator role");
                return;
            }

            await _clientHandler.MakeMemberMod(ctx.Client, member, modRole, guildId);
            //Send a confirmation
            var succesEmbed = new DiscordEmbedBuilder
            {
                Title = $"Granted Moderator Permissions",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = member.AvatarUrl
                }
            };
            
            succesEmbed.AddField("Member", $"{member.Mention}");
            succesEmbed.AddField("Role", $"{modRole.Mention}");

            await ctx.RespondAsync(MacintoshEmbed.Create(succesEmbed));
        }

        [Command(nameof(UnMod))]
        [Description("Make a member moderator")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task UnMod(CommandContext ctx, [Description("Discord member to make moderator")]
            DiscordMember member)
        {
            var guildId = ctx.Guild.Id;
            if (member == null)
            {
                await ctx.RespondAsync("No member specified for making moderator");
                return;
            }

            var modRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Name.ToLower().Contains("mod"));
            if (modRole == null)
            {
                await ctx.RespondAsync("Internal error, could not find moderator role");
                return;
            }

            await _clientHandler.MakeUnMod(ctx.Client, member, modRole, guildId);

            //Send a confirmation
            var succesEmbed = new DiscordEmbedBuilder
            {
                Title = $"Revoked Moderator Permissions",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = member.AvatarUrl
                }
            };
            
            succesEmbed.AddField("Member", $"{member.Mention}");

            await ctx.RespondAsync(MacintoshEmbed.Create(succesEmbed));
        }

        [Command(nameof(GrantRole))]
        [Description("Grant a role to a member based on their id")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task GrantRole(CommandContext ctx, [Description("Discord member to assign role")]
            DiscordMember member, [Description("The role to give the user")]
            DiscordRole role)
        {
            var guildId = ctx.Guild.Id;
            if (!role.Name.ToLower().Contains("moderator"))
            {
                var levelRole = await _levelRoleRepository.Get(role.Id, guildId);
                if (levelRole.status == Status.Found)
                    //If it's a levelrole, remove their other roles
                    await _clientHandler.RevokeOtherRoles(ctx.Client, member, levelRole.role, guildId);
                //Grant the role to the user
                await member.GrantRoleAsync(role);
                
                //Send a confirmation
                var succesEmbed = new DiscordEmbedBuilder
                {
                    Title = $"Granted New Role",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = member.AvatarUrl
                    }
                };
            
                succesEmbed.AddField("Member", $"{member.Mention}");
                succesEmbed.AddField("Role", $"{role.Mention}");

                await ctx.RespondAsync(MacintoshEmbed.Create(succesEmbed));
            }
            else
            {
                await ctx.RespondAsync("Use the ?makemod command instead");
            }
        }

        [Command(nameof(Groups))]
        [Description("Get a list of all groups supported by the database")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Groups(CommandContext ctx)
        {
            var guildId = ctx.Guild.Id;
            //Get the groups
            var groups = await _groupRepository.Get(guildId);
            //Start the string builder
            var builder = new StringBuilder();
            //Append it with the title
            builder.Append("**The list of supported groups is:**\n");
            //iterate over the groups
            foreach (var group in groups)
            {
                //append the name of the group to the string builder
                builder.Append(group.Name);
                //If it's not the last item, include a comma.
                if (!groups.Last().Equals(group)) builder.Append(", ");
            }

            //Send the group list to the member in a DM.
            await ctx.Member.SendMessageAsync(builder.ToString());
        }

        [Command(nameof(CreateGame))]
        [Description(
            "Create roles and channels for a new game\nInstructions:\n1. Set up a new emoji for the given role")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task CreateGame(CommandContext ctx,
            [Description("Emoji for the game")] DiscordEmoji emoji, [Description("Abreviation of the game")]
            string name,
            [Description(
                "Full name of the game\n(this is optional, if the full name is not specified, the role will be called <abreviation>")]
            [RemainingText]
            string fullName = null)
        {
            await CreateGroup(ctx, emoji, name, true, fullName);
        }

        [Command(nameof(CreateGroup))]
        [Description(
            "Create roles and channels for a new group\nInstructions:\n1. Set up a new emoji for the given role")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task CreateGroup(CommandContext ctx,
            [Description("Emoji for the hangout")] DiscordEmoji emoji, [Description("Abreviation of the hangout")]
            string name,
            [Description(
                "Full name of the hangout\n(this is optional, if the full name is not specified, the role will be called <abreviation>")]
            [RemainingText]
            string fullName = null)
        {
            await CreateGroup(ctx, emoji, name, false, fullName);
        }

        private async Task CreateGroup(CommandContext ctx, DiscordEmoji emoji, string name, bool isGame,
            string fullName = null)
        {
            var guildId = ctx.Guild.Id;
            //Send a working on it message and save it so we can modify it later. 
            var message = await ctx.RespondAsync("Working on it...");
            //Create the role
            var role = await CreateGroupRole(ctx, name, isGame);
            //Check if it did not get created
            if (role == null)
            {
                await message.ModifyAsync("Uknown error when creating role");
                return;
            }

            //Create the DTO's
            //group
            var group = new GroupDTO
            {
                Name = name,
                GuildId = guildId,
                FullName = fullName,
                IsGame = isGame,
                EmojiName = emoji.GetDiscordName(),
                DiscordRoleId = role.Id
            };
            //Insert into the database
            var createdGroup = await _groupRepository.Create(group);
            if (createdGroup.status != Status.Created)
            {
                await message.ModifyAsync("Error on inserting into database");
                return;
            }

            //Create the channel category and sub categories
            string channel;
            if (isGame)
                channel = await CreateChannelCategory(ctx, name, role);
            else
                channel = await CreateChannelHangout(ctx, name, role);
            //Check if it did not get created
            if (channel == null)
            {
                await message.ModifyAsync("Uknown error when creating channels");
                return;
            }

            //React with the given emojis corresponding to the new role
            var errorMessage = await ReactWithNewRoleEmoji(ctx, group);
            if (errorMessage != null)
            {
                await message.ModifyAsync(errorMessage);
                return;
            }

            //Find the role channel so the bot can mention it
            var roleChannel = await _channelRepository.Get("role", guildId);
            if (roleChannel.status != Status.Found)
            {
                await message.ModifyAsync("Could not find the roles channel in the database");
                return;
            }

            var rolesChannel = ctx.Guild.Channels.Values.FirstOrDefault(c => c.Id == roleChannel.channel.ChannelId);
            if (rolesChannel == null)
            {
                await message.ModifyAsync("Could not find the roles channel");
                return;
            }

            //Send a confirmation
            var succesEmbed = new DiscordEmbedBuilder
            {
                Title = $"Created {(isGame ? "Game" : "Group")}",
                Description = $"Get access to the channel by reacting with {emoji} in {rolesChannel.Mention}."
            };
            
           succesEmbed.AddField("Channel", $"{channel}", true);
           succesEmbed.AddField("Role", $"{role.Mention}", true);
           succesEmbed.AddField("Emoji", $"{emoji}", true);
           
           await message.ModifyAsync(MacintoshEmbed.Create(succesEmbed));
        }

        [Command(nameof(RemoveGroup))]
        [Description("Remove roles and channels for a group")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveGroup(CommandContext ctx, [Description("Abreviation of the group")]
            string name)
        {
            var guildId = ctx.Guild.Id;
            //Send a working on it message and save it so we can modify it later. 
            var message = await ctx.RespondAsync("Working on it...");
            //Get the group
            var group = await _groupRepository.Get(name, guildId);
            if (group.status != Status.Found)
            {
                await message.ModifyAsync($"Could not find {name} in the database. Have you tried running ?groups");
                return;
            }

            //Get the role
            var role = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Id == group.group.DiscordRoleId);
            if (role == null)
            {
                await message.ModifyAsync(
                    $"Could not find the role matching the group abbreviation: {group.group.Name}");
                return;
            }

            //Get the channel
            var channel = ctx.Guild.Channels.Values.FirstOrDefault(c =>
                c.Name.ToLower().Contains(group.group.Name.ToLower()) && c.IsCategory);
            if (channel == null)
            {
                await message.ModifyAsync($"Could not find the channel for {name}");
                return;
            }

            //Delete first the subchannels, and then the channel category
            foreach (var subChannel in channel.Children)
                await subChannel.DeleteAsync($"Channel deleted by user {ctx.Member.DisplayName}");
            await channel.DeleteAsync($"Group deleted by user {ctx.Member.DisplayName}");
            //Delete the role associated, save the name of it for later (notifying the chat)
            var roleName = role.Name;
            await role.DeleteAsync($"Role deleted by user {ctx.Member.DisplayName}");
            //Delete it from the repository
            var status = await _groupRepository.Delete(name, guildId);
            if (status != Status.Deleted)
            {
                await message.ModifyAsync($"Some error occured, did not remove {group.group.Name} from the database");
                return;
            }

            //Remove the part of the reaction message that is valid for this role
            var errorMessage = await RemoveEmojiReactions(ctx, group.group);
            if (errorMessage != null)
            {
                await message.ModifyAsync(errorMessage);
                return;
            }

            //Send a confirmation
            var succesEmbed = new DiscordEmbedBuilder
            {
                Title = $"Removed {(group.group.IsGame ? "Game" : "Group")}",
            };
            
            succesEmbed.AddField("Name", group.group.Name);
            
            await message.ModifyAsync(MacintoshEmbed.Create(succesEmbed));
        }

        private async Task<DiscordRole> CreateGroupRole(CommandContext ctx, string name, bool isGame)
        {
            var roleName = $"{name.Substring(0, 1).ToUpper() + name.Substring(1)}";
            if (isGame) roleName += " Gamer";
            //Create the role with the given name
            var role = await ctx.Guild.CreateRoleAsync(roleName);
            //Return it
            return role;
        }

        private async Task<string> CreateChannelCategory(CommandContext ctx, string name, DiscordRole newRole)
        {
            var overwriteBuilderList = CreateOverWritePermissions(ctx, newRole);
            //Create the channel with the given name, and fixed subcategories. 
            var newChannel =
                await ctx.Guild.CreateChannelCategoryAsync($"------{name.ToUpper()}------", overwriteBuilderList);
            await ctx.Guild.CreateTextChannelAsync($"{DiscordEmoji.FromName(ctx.Client, ":speech_left:")}{name}-chat",
                newChannel);
            await ctx.Guild.CreateVoiceChannelAsync(
                $"{DiscordEmoji.FromName(ctx.Client, ":video_game:")}{name.ToUpper()} ROOM 1", newChannel);
            await ctx.Guild.CreateVoiceChannelAsync(
                $"{DiscordEmoji.FromName(ctx.Client, ":game_die:")}{name.ToUpper()} ROOM 2", newChannel);
            //Return the uppercase name.
            return $"{name.ToUpper()}";
        }

        private async Task<string> CreateChannelHangout(CommandContext ctx, string name, DiscordRole newRole)
        {
            var overwriteBuilderList = CreateOverWritePermissions(ctx, newRole);
            //Create the channel with the given name, and fixed subcategories. 
            var newChannel =
                await ctx.Guild.CreateChannelCategoryAsync($"------{name.ToUpper()}------", overwriteBuilderList);
            await ctx.Guild.CreateTextChannelAsync($"{DiscordEmoji.FromName(ctx.Client, ":pencil:")}chat", newChannel);
            await ctx.Guild.CreateVoiceChannelAsync($"{DiscordEmoji.FromName(ctx.Client, ":telephone_receiver:")}talk",
                newChannel);
            //Return the uppercase name.
            return $"{name.ToUpper()}";
        }

        private IEnumerable<DiscordOverwriteBuilder> CreateOverWritePermissions(CommandContext ctx, DiscordRole newRole)
        {
            var overwriteBuilderList = new List<DiscordOverwriteBuilder>();
            //Deny access for everyone without moderator privelages.
            //For now, this is a workaround, since DSharp does not have functionality for making a server private. 
            foreach (var role in ctx.Guild.Roles.Values)
                if (role.Permissions.HasPermission(Permissions.ManageChannels) || role == newRole)
                {
                    //Set up the permissions for the channel
                    var overwriteBuilderAllow = new DiscordOverwriteBuilder
                    {
                        //allow access and speak
                        Allowed = Permissions.AccessChannels
                                  | Permissions.Speak
                    };
                    //allow it for the specified roles
                    overwriteBuilderAllow.For(role);
                    //Add the overwrite to the list
                    overwriteBuilderList.Add(overwriteBuilderAllow);
                }

            return overwriteBuilderList;
        }

        private async Task<string> ReactWithNewRoleEmoji(CommandContext ctx, GroupDTO group)
        {
            var guildId = ctx.Guild.Id;
            //Get the "roles" channel
            var roleChannel = await _channelRepository.Get("role", guildId);
            if (roleChannel.status != Status.Found) return "Could not find the `roles` channel in the database";
            var roleDiscordChannel =
                ctx.Guild.Channels.Values.FirstOrDefault(channel => channel.Id == roleChannel.channel.ChannelId);
            if (roleDiscordChannel == null) return "Could not find the `roles` channel";
            //Get the specific reaction message
            var message = await _messageRepository.GetMessageId("role", guildId);
            if (message.status != Status.Found) return "Could not find the `roles` message in the database";
            var assignMessage = await roleDiscordChannel.GetMessageAsync(message.messageId);
            if (assignMessage == null) return "Could not find the role assignment message";
            //Modify the message with the newly added role / group
            var discordMessage = await _clientHandler.GetReactionMessage(ctx.Client, guildId);
            await assignMessage.ModifyAsync(discordMessage);
            //React to it so users can find it easily'
            await assignMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, group.EmojiName));
            return null;
        }

        private async Task<string> RemoveEmojiReactions(CommandContext ctx, GroupDTO group)
        {
            var guildId = ctx.Guild.Id;
            //Get the "roles" channel
            var roleChannel = await _channelRepository.Get("role", guildId);
            if (roleChannel.status != Status.Found) return "Could not find the `roles` channel in the database";
            var roleDiscordChannel =
                ctx.Guild.Channels.Values.FirstOrDefault(channel => channel.Id == roleChannel.channel.ChannelId);
            if (roleDiscordChannel == null) return "Could not find the `roles` channel";
            //Get the specific reaction message
            var message = await _messageRepository.GetMessageId("role", guildId);
            if (message.status != Status.Found) return "Could not find the `roles` message in the database";
            var assignMessage = await roleDiscordChannel.GetMessageAsync(message.messageId);
            if (assignMessage == null) return "Could not find the role assignment message";
            //Modify the message with the newly added role / group
            var discordMessage = await _clientHandler.GetReactionMessage(ctx.Client, guildId);
            await assignMessage.ModifyAsync(discordMessage);

            //Remove that reaction
            await assignMessage.DeleteReactionsEmojiAsync(DiscordEmoji.FromName(ctx.Client, group.EmojiName));

            return null;
        }
    }
}