using System;
using System.Threading.Tasks;
using MacintoshBot.Models;
using MacintoshBot.Models.VoiceState;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MacintoshBot.Jobs
{
    public class CleanupJob : IJob
    {
        private readonly IVoiceStateRepository _voiceStateRepository;
        private readonly ILogger<CleanupJob> _logger;

        public CleanupJob(IVoiceStateRepository voiceStateRepository, ILogger<CleanupJob> logger)
        {
            _voiceStateRepository = voiceStateRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var oldVoiceStates = await _voiceStateRepository.EnteredMoreThanNHoursAgo(24);
            foreach (var oldVoiceState in oldVoiceStates)
            {
                _logger.LogInformation($"Got old voice state entered at {oldVoiceState.EnteredTime} for user with id: {oldVoiceState.UserId}");
                var stats = await _voiceStateRepository.Delete(oldVoiceState.Id);
                if (stats == Status.Deleted)
                {
                    _logger.LogInformation($"Deleted old voice state entered at {oldVoiceState.EnteredTime} for user with id: {oldVoiceState.UserId}");
                }
                else
                {
                    _logger.LogError($"Error when deleting old voice state entered at {oldVoiceState.EnteredTime} for user with id: {oldVoiceState.UserId}");
                }
            }
            _logger.LogDebug($"Ran: {nameof(RoleUpdateJob)}");
        }
    }
}