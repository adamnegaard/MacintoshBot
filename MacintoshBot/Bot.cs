using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using MacintoshBot.ClientHandler;
using MacintoshBot.Commands;
using MacintoshBot.Commands.Riot;
using MacintoshBot.Commands.Steam;
using MacintoshBot.Models.Channel;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;
using MacintoshBot.XpHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MacintoshBot
{
    public partial class Bot : IHostedService, IDisposable
    {
        private readonly DiscordClient _client;
        private readonly CommandsNextExtension _commands;
        private readonly SlashCommandsExtension _slash;
        private readonly IChannelRepository _channelRepository;
        private readonly IClientHandler _clientHandler;
        private readonly IGroupRepository _groupRepository;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IXpGrantModel _xpGrantModel;
        private readonly LavalinkConfiguration _lavalinkConfiguration; 
        private readonly ILogger<Bot> _logger;

        public Bot(IServiceProvider services, DiscordClient client, IUserRepository userRepository,
            IGroupRepository groupRepository,
            IMessageRepository messageRepository, ILevelRoleRepository levelRoleRepository,
            IChannelRepository channelRepository, IXpGrantModel xpGrantModel, IClientHandler clientHandler,
            ILogger<Bot> logger)
        {
            _client = client;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _levelRoleRepository = levelRoleRepository;
            _channelRepository = channelRepository;
            _xpGrantModel = xpGrantModel;
            _clientHandler = clientHandler;
            _logger = logger;

            //Get the prefix object
            var discordConfig = services.GetService<ClientConfig>();
            if (discordConfig == null)
                throw new InvalidOperationException(
                    "Add discord configuration to the dependencies");

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] {discordConfig.Prefix},
                EnableDms = true,
                EnableMentionPrefix = true,
                Services = services
            };

            var slashCommandsConfig = new SlashCommandsConfiguration
            {
                Services = services
            };

            // adding hooks
            _client.Ready += OnClientReady;
            _client.MessageReactionAdded += OnReactionAdded;
            _client.MessageReactionRemoved += OnReactionRemoved;
            _client.GuildMemberUpdated += OnGuildMemberUpdated;
            _client.GuildMemberRemoved += OnMemberRemoved;
            _client.GuildAvailable += OnGuildAvailable;
            _client.VoiceStateUpdated += OnVoiceStateUpdate;
            _client.GuildRoleUpdated += OnGuildRoleUpdated;
            
            // slash commands adding
            _slash = _client.UseSlashCommands(slashCommandsConfig);
            _slash.RegisterCommands<LevelCommands>();
            _slash.RegisterCommands<ManageCommands>();

            // commands adding
            _commands = _client.UseCommandsNext(commandsConfig);
            _commands.RegisterCommands<ConnectionCommands>();
            _commands.RegisterCommands<RustCommands>();
            _commands.RegisterCommands<CsCommands>();
            _commands.RegisterCommands<LeagueCommands>();
            _commands.RegisterCommands<RandomCommands>();
            _commands.RegisterCommands<MusicCommands>();
            
            //Get the config object for lavalink
            var lavalinkConfig = services.GetService<LavalinkConfig>(); 
            if (lavalinkConfig == null)
                throw new InvalidOperationException(
                    "Add Lavalink configuration to the dependencies");
            
            var endpoint = new ConnectionEndpoint
            {
                Hostname = lavalinkConfig.Host,
                Port = int.Parse(lavalinkConfig.Port)
            };

            _lavalinkConfiguration = new LavalinkConfiguration
            {
                Password = lavalinkConfig.Password,
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
                
            };
        }
        
        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot starting...");
            await _client.ConnectAsync(
                new DiscordActivity("?help", ActivityType.ListeningTo),
                UserStatus.Online);
            
            _logger.LogInformation("Lavalink starting...");
            try
            {
                //var lavalink = _client.UseLavalink();
                //await lavalink.ConnectAsync(_lavalinkConfiguration);
            }
            catch (WebSocketException e)
            {
                _logger.LogError("Not able to connect to lavalink", e);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot stopping...");
            await _client.DisconnectAsync();
        }

        private static Task OnClientReady(DiscordClient client, ReadyEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }
    }
}