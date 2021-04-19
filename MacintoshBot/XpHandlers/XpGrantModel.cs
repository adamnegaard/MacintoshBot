using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Models;
using MacintoshBot.Models.User;

namespace MacintoshBot.XpHandlers
{
    public class XpGrantModel : IXpGrantModel
    {
        private readonly IUserRepository _userRepository;
        private readonly HashSet<(ulong memberId, ulong guildId, DateTime time)> voiceTimeSet;

        public XpGrantModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            voiceTimeSet = new HashSet<(ulong memberId, ulong guildId, DateTime time)>();
        }


        public void EnterVoiceChannel(ulong memberId, ulong guildId)
        {
            voiceTimeSet.Add((memberId, guildId, DateTime.Now));
        }

        public async Task<int> ExitVoiceChannel(ulong memberId, ulong guildId)
        {
            var member = voiceTimeSet.FirstOrDefault(p => p.memberId == memberId && p.guildId == guildId);
            if (member.time.Equals(DateTime.MinValue))
                //Exception should be thrown here
                return 0;

            var newXp = await GetNewXpFromStartTime(member.time, memberId, guildId);
            //Remove the user from the set timing it
            voiceTimeSet.Remove(member);
            return newXp;
        }

        public async Task<int> GetNewXpFromStartTime(DateTime startTime, ulong memberId, ulong guildId)
        {
            var now = DateTime.Now;
            if (startTime > now)
            {
                var member = await _userRepository.Get(memberId, guildId);
                if (member.status != Status.Found) return 0;

                return member.user.Xp;
            }

            var timeInChat = (now - startTime).TotalMinutes;
            return await _userRepository.AddXp(memberId, guildId, Convert.ToInt32(timeInChat * 5));
        }
    }
}