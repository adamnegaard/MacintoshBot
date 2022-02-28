using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using Microsoft.Extensions.Logging;
using NLog;

namespace MacintoshBot.Commands
{
    [Description("Roles related to playing music")]
    public class MusicCommands : BaseCommandModule
    {

        private readonly ILogger<MusicCommands> _logger;

        public MusicCommands(ILogger<MusicCommands> logger)
        {
            _logger = logger;
        }

        [Command(nameof(Join))]
        [Description("Join the current channel the user is in")]
        public async Task Join(CommandContext ctx)
        {
            if (! await InVoiceChannel(ctx))
            {
                return;
            }

            var channel = ctx.Member.VoiceState.Channel;

            var conn = await JoinChannel(ctx, channel);

            if (conn != null)
            {
                var message = await ctx.RespondAsync($"Joined {channel.Name}!");
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":wave:"));
            }
        }

        [Command(nameof(Leave))]
        [Description("Leave the current channel if the user and bot are in the same channel")]
        public async Task Leave(CommandContext ctx)
        {
            if (! await InVoiceChannel(ctx))
            {
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
                await ctx.RespondAsync("Not a valid voice channel");
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected");
                return;
            }

            await conn.DisconnectAsync();
            
            _logger.LogInformation($"{ctx.Member.DisplayName} succesfully disconnected the bot");
            
            var message = await ctx.RespondAsync($"Bye!");
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":wave:"));
        }

        [Command(nameof(Play))]
        [Description("Play the specified song")]
        public async Task Play(CommandContext ctx, [Description("The song to play")] [RemainingText] string search)
        {
            var memberChannel = ctx.Member.VoiceState.Channel;
            
            _logger.LogInformation($"{ctx.Member.DisplayName} searched for {search}");
            
            if (! await InVoiceChannel(ctx))
            {
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

            if (! await InSameChannel(conn, ctx))
            {
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed 
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}");
                _logger.LogInformation($"{ctx.Member.DisplayName}'s search for {search} gave no results");
                return;
            }

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);
            
            // leave the channel when finished playing song
            node.PlaybackFinished += (conn, eventArgs) => OnPlaybackFinished(conn, eventArgs, ctx);
            
            _logger.LogInformation($"{ctx.Member.DisplayName} played {track.Title}");

            var message = await ctx.RespondAsync(GetTrackEmbed("Now playing", track));
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":musical_note:"));
        }

        [Command(nameof(Pause))]
        [Description("Pause the current playing song")]
        public async Task Pause(CommandContext ctx)
        {
            if (! await InVoiceChannel(ctx))
            {
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
            
            if (! await InSameChannel(conn, ctx))
            {
                return;
            }
            
            _logger.LogInformation($"{ctx.Member.DisplayName} paused {currentTrack.Title}");

            await conn.PauseAsync();
            
            var message = await ctx.RespondAsync(GetTrackEmbed("Paused", currentTrack));
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":pause_button:"));
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
            
            if (! await InSameChannel(conn, ctx))
            {
                return;
            }

            await conn.ResumeAsync();
            
            _logger.LogInformation($"{ctx.Member.DisplayName} resumed {currentTrack.Title}");
            
            var message = await ctx.RespondAsync(GetTrackEmbed("Resumed", currentTrack));
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":play_pause:"));
        }
        
        private DiscordEmbed GetTrackEmbed(string trackPrefix, LavalinkTrack track)
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

        private async Task<LavalinkGuildConnection> JoinChannel(CommandContext ctx, DiscordChannel channel)
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
            
            _logger.LogInformation($"{ctx.Member.DisplayName} succesfully connected the bot");

            return await node.ConnectAsync(channel);
        }

        private async Task<bool> InSameChannel(LavalinkGuildConnection conn, CommandContext ctx)
        {
            if (conn.Channel.Id != ctx.Member.VoiceState.Channel.Id)
            {
                await ctx.RespondAsync($"Join {conn.Channel.Name} to control music");
                return false;
            }

            return true;
        }
        
        private async Task<bool> InVoiceChannel(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel");
                return false;
            }

            return true;
        }

        private async Task OnPlaybackFinished(LavalinkGuildConnection conn, TrackFinishEventArgs eventArgs, CommandContext ctx)
        {
            var track = eventArgs.Track;
            _logger.LogInformation($"Finished playing {track.Title}");
            _logger.LogInformation($"Leaving channel {conn.Channel.Name} after finishing track");
            await conn.DisconnectAsync();

            var discordEmbed = new DiscordEmbedBuilder
            {
                Title = $"Finished playing {track.Title}",
            };

            var message = await ctx.Channel.SendMessageAsync(MacintoshEmbed.Create(discordEmbed));
            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":wave:"));
        }
    }
}