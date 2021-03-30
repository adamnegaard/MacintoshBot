using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.Models.Facts;
using MacintoshBot.Models.Image;

namespace MacintoshBot.Commands
{
    //[Group("Random")]
    [Description("Random and fun commands!")]
    public class RandomCommands : BaseCommandModule
    {
        
        private readonly IImageRepository _imageRepository;
        private readonly IFactRepository _factRepository;
        private readonly IClientHandler _clientHandler;

        public RandomCommands(IImageRepository imageRepository, IFactRepository factRepository, IClientHandler clientHandler)
        {
            _imageRepository = imageRepository;
            _factRepository = factRepository;
            _clientHandler = clientHandler;
        }

        [Command("roll")]
        [Description("Roll a dice from 1-6")]
        public async Task Roll(CommandContext ctx, [Description("(Optional) min value")] int min = 1, [Description("(Optional) max value")] int max = 6)
        {
            var random = new Random();
            await ctx.Channel.SendMessageAsync($"{ctx.Member.DisplayName} rolled: {random.Next(min, max)}");
        }

        [Command("Poggers")]
        [Description("A poggers image")]
        public async Task Poggers(CommandContext ctx)
        {
            var guildId = ctx.Guild.Id;
            var imageEmbed = new DiscordEmbedBuilder
            {
                Title = "Paaaawg Chaaampion",
                Description = "A wild pawgers appeared"
            };
            var poggers = await _imageRepository.Get("poggers", guildId);
            if (poggers == null)
            {
                await ctx.Channel.SendMessageAsync("Could not find the poggers image");
                return;
            }
            imageEmbed.WithImageUrl(poggers);
            await ctx.Channel.SendMessageAsync(embed: imageEmbed);
        }
        
        [Command("fact")]
        [Description("A poggers image")]
        public async Task Fact(CommandContext ctx, [Description("Fact number")] int num = 0)
        {
            var fact = await _factRepository.Get(num);
            if (fact == null)
            {
                await ctx.Channel.SendMessageAsync($"Could not find the fact with Id {num}");
                return;
            }

            await _clientHandler.CreateFactMessage(ctx.Client, fact, ctx.Channel);
        }
        
        [Command("Images")]
        [Description("See the list of available images")]
        public async Task Images(CommandContext ctx)
        {
            var guildId = ctx.Guild.Id;
            var images = await _imageRepository.Get(guildId);
            if (images == null)
            {
                await ctx.Channel.SendMessageAsync("Something went wrong");
                return;
            }
            var builder = new StringBuilder();
            builder.Append("**The list of available images is:**\n");
            foreach (var imageTitle in images)
            {
                builder.Append(imageTitle);
                if (!images.Last().Equals(imageTitle)) builder.Append(", ");
            }

            await ctx.Member.SendMessageAsync(builder.ToString()); 
        }
        
        [Command("Show")]
        [Description("Show a image based on its string representation")]
        public async Task Show(CommandContext ctx, [Description("Name of image")] string imageTitle)
        {
            var guildId = ctx.Guild.Id;
            var image = await _imageRepository.Get(imageTitle, guildId);
            if (image == null)
            {
                await ctx.Channel.SendMessageAsync($"Could not find the image {imageTitle}, try `?images`");
                return;
            }
            var imageEmbed = new DiscordEmbedBuilder
            {
                Title = imageTitle,
            };
            imageEmbed.WithImageUrl(image);
            await ctx.Channel.SendMessageAsync(embed: imageEmbed);
        }
    }
}