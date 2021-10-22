using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MacintoshBot.Models.User;
using RiotSharp;
using RiotSharp.Endpoints.ChampionMasteryEndpoint;
using RiotSharp.Endpoints.StaticDataEndpoint;
using RiotSharp.Endpoints.StaticDataEndpoint.ProfileIcons;
using RiotSharp.Endpoints.SummonerEndpoint;
using RiotSharp.Interfaces;
using RiotSharp.Misc;

namespace MacintoshBot.Commands.Riot
{
    [System.ComponentModel.Description("Commands related to League of Legends")]
    public class LeagueCommands : RiotCommandBase
    {
        public LeagueCommands(IUserRepository userRepository, IRiotApi riotApi) : base(
            userRepository, riotApi)
        { }
        
        [Command(nameof(LoLStats))]
        [Description("Check your League of Legends stats")]
        public async Task LoLStats(CommandContext ctx, DiscordMember member = null)
        {
            var (user, loadingMessage, taggedMember) = await GetStatsMessageAndUser(ctx, member);
            if(user == null)
            {
                // inform is sent in function call above
                return;
            }

            try
            {
                var summonerName = user.SummonerName;
                if (summonerName == null)
                {
                    await loadingMessage.ModifyAsync($"{taggedMember.DisplayName} does not have a SummonerName set");
                    return;
                }

                // summoner
                var summoner = GetSummonerByNameInEuRegions(summonerName);
                if (summoner == null)
                {
                    await loadingMessage.ModifyAsync($"{summonerName} was not found in the Riot API");
                    return;
                }

                // summoner icon
                var imageStatic = await GetSummonerImage(summoner.ProfileIconId);
                var imageLocation = imageStatic != null
                    ? $"http://ddragon.leagueoflegends.com/cdn/{version}/img/profileicon/{imageStatic.Full}"
                    : null;

                var discordEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = summoner.Name,
                        IconUrl = imageLocation
                    },
                    Title = $"{summoner.Name}'s LoL stats",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = imageLocation,
                    }
                };

                discordEmbed.AddField("Level", summoner.Level.ToString(), true);
                discordEmbed.AddField("Region", summoner.Region.ToString());

                // masteries
                var masteries = await _riotApi.ChampionMastery.GetChampionMasteriesAsync(summoner.Region, summoner.Id);

                discordEmbed = await ReadChampionStats(discordEmbed, masteries);

                await loadingMessage.ModifyAsync(MacintoshEmbed.Create(discordEmbed));
            }
            catch (Exception e)
            {
                await loadingMessage.ModifyAsync("Unknown error when fetching from the Riot API");
            }
           
        }

        private async Task<DiscordEmbedBuilder> ReadChampionStats(DiscordEmbedBuilder discordEmbed, List<ChampionMastery> masteries)
        {
            var champions = await _riotApi.StaticData.Champions.GetAllAsync(version);
            // Get the top 3 masteries
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var mastery = masteries[i];
                    // get the champion info
                    var champion = champions.Keys.FirstOrDefault(k => k.Key == mastery.ChampionId);
                    if (champion.Key == 0 || champion.Value == null)
                    {
                        continue;
                    }
                    discordEmbed.AddField("Champion", champion.Value, true);
                    discordEmbed.AddField("Mastery Level", mastery.ChampionLevel.ToString(), true);
                    discordEmbed.AddField("Mastery Points", mastery.ChampionPoints.ToString(), true);
                }
                catch (RiotSharpException riotSharpException)
                {
                    // skip the given mastery
                }       
            }
            return discordEmbed;
        }


        private async Task<ImageStatic> GetSummonerImage(int profileIconId)
        {
            var icons = await _riotApi.StaticData.ProfileIcons.GetAllAsync(version);
            var iconStatic = icons.ProfileIcons.Values.FirstOrDefault(icon => icon.Id == profileIconId);
            if (iconStatic == null)
            {
                return null;
            }
            return iconStatic.Image;
        }
        private Summoner GetSummonerByNameInEuRegions(string summonerName)
        {
            // prefer EUW first, possibly fix this by saving the region on the user entity?
            var euRegions = new[] { Region.Euw, Region.Eune };
            foreach (var region in euRegions)
            {
                try
                {
                    var summoner = _riotApi.Summoner.GetSummonerByNameAsync(region, summonerName).Result;
                    if (summoner == null)
                    {
                        continue;
                    }

                    return summoner;
                }
                catch (RiotSharpException ex)
                {
                    // skip the given region
                }
            }
            return null;
        }
    }
}