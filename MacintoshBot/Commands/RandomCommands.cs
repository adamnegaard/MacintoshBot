﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.ClientHandler;
using MacintoshBot.Models;
using MacintoshBot.Models.Facts;
using MacintoshBot.Models.File;

namespace MacintoshBot.Commands
{
    //[Group("Random")]
    [Description("Random and fun commands!")]
    public class RandomCommands : BaseCommandModule
    {
        private readonly IClientHandler _clientHandler;
        private readonly IFactRepository _factRepository;

        private readonly IFileRepository _fileRepository;

        public RandomCommands(IFileRepository fileRepository, IFactRepository factRepository,
            IClientHandler clientHandler)
        {
            _fileRepository = fileRepository;
            _factRepository = factRepository;
            _clientHandler = clientHandler;
        }

        [Command(nameof(Roll))]
        [Description("Roll a dice from 1-6")]
        public async Task Roll(CommandContext ctx, [Description("(Optional) min value")] int min = 1,
            [Description("(Optional) max value")] int max = 7)
        {
            var random = new Random();
            
            //Send a confirmation
            var roleEmbed = new DiscordEmbedBuilder
            {
                Title = $"Rolled",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Member.AvatarUrl
                }
            };
            
            roleEmbed.AddField("Value", $"{random.Next(min, max)}");

            await ctx.RespondAsync(MacintoshEmbed.Create(roleEmbed));
        }

        [Command(nameof(Fact))]
        [Description("Get on of the facts from the #fact channel")]
        public async Task Fact(CommandContext ctx, [Description("Fact number")] int num = 0)
        {
            var fact = await _factRepository.Get(num);
            if (fact.status != Status.Found)
            {
                await ctx.RespondAsync($"Could not find the fact with Id {num}");
                return;
            }

            await _clientHandler.CreateFactMessage(ctx.Client, fact.fact, ctx.Channel);
        }

        [Command(nameof(Files))]
        [Description("See the list of available files")]
        public async Task Files(CommandContext ctx)
        {
            var guildId = ctx.Guild.Id;
            var files = await _fileRepository.Get(guildId);
            if (files == null)
            {
                await ctx.RespondAsync("Something went wrong");
                return;
            }

            var builder = new StringBuilder();
            builder.Append("**The list of available files is:**\n");
            foreach (var fileTitle in files)
            {
                builder.Append(fileTitle);
                if (!files.Last().Equals(fileTitle)) builder.Append(", ");
            }

            await ctx.Member.SendMessageAsync(builder.ToString());
        }

        [Command(nameof(Get))]
        [Description("Show a file based on its string representation")]
        public async Task Get(CommandContext ctx, [Description("Name of file")] [RemainingText]
            string fileTitle)
        {
            var guildId = ctx.Guild.Id;
            var response = await _fileRepository.Get(fileTitle, guildId);
            if (response.status != Status.Found)
            {
                await ctx.RespondAsync(
                    $"Could not find the file {fileTitle} in the database, try `?files`");
                return;
            }

            var file = response.file;
            var imageEmbed = new DiscordEmbedBuilder
            {
                Title = file.Title,
                ImageUrl = file.Location
            };
            
            await ctx.RespondAsync(MacintoshEmbed.Create(imageEmbed));
        }

        [Command(nameof(AddFile))]
        [Description("Adds an file to the available files on the server")]
        public async Task AddFile(CommandContext ctx, [Description("Name of file")] [RemainingText]
            string fileTitle)
        {
            var message = ctx.Message;
            var guildId = ctx.Guild.Id;
            var attachments = message.Attachments;
            
            var attachment = attachments.FirstOrDefault();
            if (attachment == null)
            {
                await ctx.RespondAsync("Please attach something!");
                return;
            }

            var file = new FileDTO
            {
                Title = fileTitle,
                GuildId = guildId,
                Location = attachment.Url
            };

            var response = await _fileRepository.Create(file);
            if (response.status != Status.Created)
            {
                await ctx.RespondAsync("Uknown error, could not create the file");
                return;
            }
            
            var roleEmbed = new DiscordEmbedBuilder
            {
                Title = $"Added file",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = file.Location
                }
            };
        
            roleEmbed.AddField("Title", $"{fileTitle}");

            await ctx.RespondAsync(MacintoshEmbed.Create(roleEmbed));
        }
    }
}