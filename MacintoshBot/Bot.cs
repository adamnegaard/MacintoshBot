using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using MacintoshBot.Commands;
using MacintoshBot.Entities;
using MacintoshBot.Models.Group;
using MacintoshBot.Models.Message;
using MacintoshBot.Models.Role;
using MacintoshBot.Models.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace MacintoshBot
{
    public partial class Bot : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private DiscordClient _client { get; set; }
        private CommandsNextExtension _commands { get; set; }
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILevelRoleRepository _levelRoleRepository;
        private readonly IXpGrantModel _xpGrantModel;
        private readonly IClientHandler _clientHandler;

        public Bot(IServiceProvider services, DiscordClient client, IUserRepository userRepository, IGroupRepository groupRepository, 
            IMessageRepository messageRepository, ILevelRoleRepository levelRoleRepository, IXpGrantModel xpGrantModel, IClientHandler clientHandler)
        {
            _services = services;
            _client = client;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _levelRoleRepository = levelRoleRepository;
            _xpGrantModel = xpGrantModel;
            _clientHandler = clientHandler;
            
            //Get the prefix object
            var config = _services.GetService<ClientConfig>();
            if (config == null)
            {
                throw new InvalidOperationException(
                    "Add a ClientConfig to the dependencies");
                    
            }

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new [] {config.Prefix},
                EnableDms = true,
                EnableMentionPrefix = true,
                Services = services
            };

            //adding hooks
            _client.Ready += OnClientReady;
            _client.MessageReactionAdded += OnReactionAdded;
            _client.MessageReactionRemoved += OnReactionRemoved;
            _client.GuildMemberAdded += OnMemberJoined;
            _client.GuildMemberRemoved += OnMemberRemoved;
            _client.GuildAvailable += OnGuildAvailable;
            _client.VoiceStateUpdated += OnVoiceStateUpdate;

            //Commands adding
            _commands = _client.UseCommandsNext(commandsConfig);
            _commands.RegisterCommands<ManageCommands>();
            _commands.RegisterCommands<LevelCommands>();
            _commands.RegisterCommands<RandomCommands>();
        }
        
        private Task OnClientReady(DiscordClient client, ReadyEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync(
                new DiscordActivity("?help", ActivityType.ListeningTo),
                UserStatus.Online).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}