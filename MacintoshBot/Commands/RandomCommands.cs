using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.Models.Image;

namespace MacintoshBot.Commands
{
    //[Group("Random")]
    [Description("Random and fun commands!")]
    public class RandomCommands : BaseCommandModule
    {
        
        private readonly IImageRepository _repository;

        public RandomCommands(IImageRepository repository)
        {
            _repository = repository;
        }
        
        [Command("roll")]
        [Description("Roll a dice from 1-6")]
        public async Task Roll(CommandContext ctx, [Description("(Optional) min value")] int min = 1, [Description("(Optional) max value")] int max = 6)
        {
            var random = new Random();
            await ctx.Channel.SendMessageAsync($"{ctx.Member.DisplayName} rolled: {random.Next(min, max)}");
        }

        [Command("poggers")]
        [Description("A poggers image")]
        public async Task Poggers(CommandContext ctx)
        {
            var imageEmbed = new DiscordEmbedBuilder
            {
                Title = "Paaaawg Chaaampion",
                Description = "A wild pawgers appeared"
            };
            imageEmbed.WithImageUrl(await _repository.Get("poggers"));
            await ctx.Channel.SendMessageAsync(embed: imageEmbed);
        }
        
        [Command("images")]
        [Description("See the list of available images")]
        public async Task Images(CommandContext ctx)
        {
            var images = await _repository.Get();
            var builder = new StringBuilder();
            builder.Append("The list of available images is:\n");
            foreach (var imageTitle in images)
            {
                builder.Append(imageTitle);
                if (!images.Last().Equals(imageTitle)) builder.Append(", ");
            }

            await ctx.Member.SendMessageAsync(builder.ToString()); 
        }
        
        [Command("show")]
        [Description("Show a image based on its string representation")]
        public async Task ShowImage(CommandContext ctx, [Description("Name of image")] string imageTitle)
        {
            var image = await _repository.Get(imageTitle);
            if (image == null)
            {
                await ctx.Channel.SendMessageAsync($"Could not find the image {imageTitle}, try ?images");
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