using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace MacintoshBot.Models.VoiceState
{
    public class VoiceStateRepository : IVoiceStateRepository
    {
        private readonly IDiscordContext _context;
        private readonly ILogger<VoiceStateRepository> _logger;

        public VoiceStateRepository(IDiscordContext context, ILogger<VoiceStateRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(Status status, Entities.VoiceState message)> Create(VoiceStateCreate voiceStateCreate)
        {
            try
            {
                var voiceState = new Entities.VoiceState(voiceStateCreate);

                await _context.VoiceStates.AddAsync(voiceState);
                await _context.SaveChangesAsync();

                return (Status.Created, voiceState);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error when creating voice state {e}");
                return (Status.Error, null); 
            }
        }

        public async Task<(Status status, Entities.VoiceState message)> Update(VoiceStateUpdate voiceStateUpdate)
        {
            try
            {
                var voiceState = await _context.VoiceStates
                    .Where(vs => vs.UserId == voiceStateUpdate.UserId)
                    .Where(vs => vs.GuildId == voiceStateUpdate.GuildId)
                    .Where(vs => vs.EnteredTime != null && vs.LeftTime == null)
                    .OrderByDescending(vs => vs.EnteredTime)
                    .FirstOrDefaultAsync();

                if (voiceState == null)
                {
                    _logger.LogError($"Attempted to update voicestate {voiceStateUpdate}, which was not in updatable state");
                    return (Status.BadRequest, null);
                }

                voiceState = voiceStateUpdate.UpdateEntity(voiceState);
                _context.VoiceStates.Update(voiceState);
                await _context.SaveChangesAsync(); 

                return (Status.Updated, voiceState);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error when updating voice state {e}");
                return (Status.Error, null); 
            }
        }
        
        public async Task<IEnumerable<Entities.VoiceState>> EnteredMoreThanNHoursAgo(int n)
        {
            try
            {
                var now = DateTime.Now;
                var nHoursAgo = now.AddHours(-n);
                return await _context.VoiceStates
                    .Where(vs => vs.EnteredTime != null && vs.EnteredTime <= nHoursAgo)
                    .Where(vs => vs.LeftTime == null)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error when reading voice states {e}");
                return new List<Entities.VoiceState>();
            }
        }

        public async Task<Status> Delete(int id)
        {
            try
            {
                var voiceState = await _context.VoiceStates.FirstOrDefaultAsync(vs => vs.Id == id);
                if (voiceState == null)
                {
                    return Status.BadRequest;
                }
                
                _context.VoiceStates.Remove(voiceState);
                await _context.SaveChangesAsync();
                return Status.Deleted;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error when deleting voice states {e}");
                return Status.Error;
            }
        }
    }
}