using FlashscoreAutomation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlashscoreAutomation.Automations
{
    public interface IAutomations
    {
        Task<List<LeagueResult>> GetLeaguesInfoAsync(List<FootballLeagueInfo> leagueInfos);
    }
}
