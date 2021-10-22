using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
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
        private readonly IChannelRepository _channelRepository;
        private readonly IClientHandler _clientHandler;
        private readonly IGroupRepository _groupRepository;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IServiceProvider _services;
        private readonly IUserRepository _userRepository;
        private readonly IXpGrantModel _xpGrantModel;
        private readonly ILogger<Bot> _logger;

        public Bot(IServiceProvider services, DiscordClient client, IUserRepository userRepository,
            IGroupRepository groupRepository,
            IMessageRepository messageRepository, ILevelRoleRepository levelRoleRepository,
            IChannelRepository channelRepository, IXpGrantModel xpGrantModel, IClientHandler clientHandler,
            ILogger<Bot> logger)
        {
            _services = services;
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
            var config = _services.GetService<ClientConfig>();
            if (config == null)
                throw new InvalidOperationException(
                    "Add a ClientConfig to the dependencies");

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] {config.Prefix},
                EnableDms = true,
                EnableMentionPrefix = true,
                Services = services
            };

            //adding hooks
            _client.Ready += OnClientReady;
            _client.MessageReactionAdded += OnReactionAdded;
            _client.MessageReactionRemoved += OnReactionRemoved;
            _client.GuildMemberUpdated += OnGuildMemberUpdated;
            _client.GuildMemberRemoved += OnMemberRemoved;
            _client.GuildAvailable += OnGuildAvailable;
            _client.VoiceStateUpdated += OnVoiceStateUpdate;
            _client.GuildRoleUpdated += OnGuildRoleUpdated;

            //Commands adding
            _commands = _client.UseCommandsNext(commandsConfig);
            _commands.RegisterCommands<ConnectionCommands>();
            _commands.RegisterCommands<RustCommands>();
            _commands.RegisterCommands<CsCommands>();
            _commands.RegisterCommands<LeagueCommands>();
            _commands.RegisterCommands<ManageCommands>();
            _commands.RegisterCommands<LevelCommands>();
            _commands.RegisterCommands<RandomCommands>();
        }

        private DiscordClient _client { get; }
        private CommandsNextExtension _commands { get; }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot started...");
            await _client.ConnectAsync(
                new DiscordActivity("?help", ActivityType.ListeningTo),
                UserStatus.Online).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot stopped...");
            await _client.DisconnectAsync().ConfigureAwait(false);
        }

        private Task OnClientReady(DiscordClient client, ReadyEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }
    }
}