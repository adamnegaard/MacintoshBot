using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace MacintoshBot.Commands
{
    [Description("Roles related to playing music")]
    public class MusicCommands : BaseCommandModule
    {
        
        [Command(nameof(Join))]
        [Description("Join the current channel the user is in")]
        public async Task Join(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var channel = ctx.Member.VoiceState.Channel;

            await JoinChannel(ctx, channel); 
            
            await ctx.RespondAsync($"Joined {channel.Name}!");
        }

        [Command(nameof(Leave))]
        [Description("Leave the current channel if the user and bot are in the same channel")]
        public async Task Leave(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var channel = ctx.Member.VoiceState.Channel; 
            
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The Lavalink connection is not established");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {channel.Name}!");
        }

        [Command(nameof(Play))]
        [Description("Play the specified song")]
        public async Task Play(CommandContext ctx, [Description("The song to play")] [RemainingText] string search)
        {
            var memberChannel = ctx.Member.VoiceState.Channel;
            if (ctx.Member.VoiceState == null || memberChannel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            
            // return if the current connection is null, and the bot was not able to join now
            if (conn == null)
            {
                conn = await JoinChannel(ctx, memberChannel); 
                if (conn == null)
                {
                    return;   
                }
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed 
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}.");
                return;
            }

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);

            await ctx.RespondAsync(GetTrackEmbed("Now playing", track));
        }

        private DiscordMessageBuilder GetTrackEmbed(string trackPrefix, LavalinkTrack track)
        {
            var discordEmbed = new DiscordEmbedBuilder
            {
                Title = $"{trackPrefix} {track.Title}",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = track.Author
                },
                Url = track.Uri.ToString()
            };
            return MacintoshEmbed.Create(discordEmbed); 
        }
        
        [Command(nameof(Pause))]
        [Description("Pause the current playing song")]
        public async Task Pause(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected");
                return;
            }

            var currentTrack = conn.CurrentState.CurrentTrack;

            if (currentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded");
                return;
            }

            await conn.PauseAsync();
            
            await ctx.RespondAsync(GetTrackEmbed("Paused", currentTrack));
        }
        
        [Command(nameof(Resume))]
        [Description("Resumes the current playing song")]
        public async Task Resume(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected");
                return;
            }

            var currentTrack = conn.CurrentState.CurrentTrack;

            if (currentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded");
                return;
            }

            await conn.ResumeAsync();
            
            await ctx.RespondAsync(GetTrackEmbed("Resumed", currentTrack));
        }

        private static async Task<LavalinkGuildConnection> JoinChannel(CommandContext ctx, DiscordChannel channel)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The connection is not established");
                return null;
            }

            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(channel.Guild);

            if (conn != null)
            {
                await ctx.RespondAsync("Already in another voice channel.");
                return null;
            }

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return null;
            }

            return await node.ConnectAsync(channel);
        }
    }
}