using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacintoshBot.Models.User;
using Microsoft.CSharp.RuntimeBinder;

namespace MacintoshBot
{
    public class XpGrantModel : IXpGrantModel
    {
        private readonly IUserRepository _repository;
        private HashSet<(ulong memberId,ulong guildId, DateTime time)> voiceTimeSet;

        public XpGrantModel(IUserRepository repository)
        {
            _repository = repository;
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
            {
                //Exception should be thrown here
                return 0;
            }
            var timeInChat = (DateTime.Now - member.time).Minutes * 5;
            var newXp = await _repository.AddXp(memberId, guildId, timeInChat);
            
            //Remove the user from the set timing it
            voiceTimeSet.Remove(member);
            return newXp;
        }
    }
}