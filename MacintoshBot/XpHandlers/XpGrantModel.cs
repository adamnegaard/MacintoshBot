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
        private HashSet<(ulong memberId, DateTime time)> voiceTimeSet;

        public XpGrantModel(IUserRepository repository)
        {
            _repository = repository;
            voiceTimeSet = new HashSet<(ulong memberId, DateTime time)>();
        }


        public void EnterVoiceChannel(ulong memberId)
        {
            voiceTimeSet.Add((memberId, DateTime.Now));
        }

        public async Task<int> ExitVoiceChannel(ulong memberId)
        {
            var member = voiceTimeSet.FirstOrDefault(p => p.memberId == memberId);
            if (member.time.Equals(DateTime.MinValue))
            {
                //Exception should be thrown here
                return 0;
            }
            var timeInChat = (DateTime.Now - member.time).Minutes * 5;
            //Remove the user from the set timing it
            voiceTimeSet.Remove(member);
            var xp = await _repository.AddXp(memberId, timeInChat);
            return xp;
        }
    }
}