using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Models;
using MacintoshBot.Models.User;
using MacintoshBot.Models.VoiceState;
using Microsoft.Extensions.Logging;
using NLog;

namespace MacintoshBot.XpHandlers
{
    public class XpGrantModel : IXpGrantModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IVoiceStateRepository _voiceStateRepository;
        private readonly ILogger<XpGrantModel> _logger;

        public XpGrantModel(IUserRepository userRepository, IVoiceStateRepository voiceStateRepository, ILogger<XpGrantModel> logger)
        {
            _userRepository = userRepository;
            _voiceStateRepository = voiceStateRepository;
            _logger = logger;
        }


        public async Task EnterVoiceChannel(ulong memberId, ulong guildId)
        {
            var voiceStateEnter = new VoiceStateCreate
            {
                UserId = memberId,
                GuildId = guildId,
                EnteredTime = DateTime.Now
            };
            
            var (status, voiceState) = await _voiceStateRepository.Create(voiceStateEnter);
        }

        public async Task<int> ExitVoiceChannel(ulong memberId, ulong guildId)
        {
            var voiceStateUpdate = new VoiceStateUpdate
            {
                UserId = memberId,
                GuildId = guildId,
                LeftTime = DateTime.Now
            };
            var (status, voiceState) = await _voiceStateRepository.Update(voiceStateUpdate);
            if (status == Status.Updated)
            {
                var timeInChat = voiceState.GetTotalMinutes;
                await _userRepository.AddXp(memberId, guildId, timeInChat);
                return timeInChat;
            }
            _logger.LogError($"Could not exit voice state for member with id {memberId} in guild {guildId}");
            return 0;
        }

        public async Task MoveVoiceChannel(ulong memberId, ulong guildId)
        {
            var voiceStateUpdate = new VoiceStateUpdate
            {
                UserId = memberId,
                GuildId = guildId,
                MovedTime = DateTime.Now
            };
            
            var (status, voiceState) = await _voiceStateRepository.Update(voiceStateUpdate);
            if (status != Status.Updated)
            {
                _logger.LogError($"Could not move voice state for member with id {memberId} in guild {guildId}");
            }
        }
    }
}