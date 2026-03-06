using System.Text.Json;
using System.Text.Json.Serialization;
using FlashscoreAutomation.Models;
using Microsoft.Playwright;
using OfficeOpenXml;

namespace FlashscoreAutomation
{

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Logger.Log("Application started");
            ExcelPackage.License.SetNonCommercialPersonal("Jakub");
            List<FootballLeagueInfo> leagueInfos = ReadFromJSON.ReadLeagueInfo();

            var results = await Automations.Automations.GetLeaguesInfoAsync(leagueInfos);
            var leaguesData = new Dictionary<string, List<(string TeamName, int MP, int W, int D, int L, string Goals, int RB, int Pts)>>();

            foreach (var league in results)
            {
                var list = new List<(string TeamName, int MP, int W, int D, int L, string Goals, int RB, int Pts)>();
                foreach (var team in league.Teams)
                {
                    list.Add((team.TeamName, team.MP, team.W, team.D, team.L, team.Goals, team.RB, team.Pts));
                }
                leaguesData.Add(league.LeagueName, list);
            }

            var temperature = new List<(string Country, double Temperature)>();

            foreach (var temp in leagueInfos)
            {
                var tempData = await Temperature.GetTemperatureAsync(temp.Latitude, temp.Longitude);
                temperature.Add((temp.Country, tempData));
            }

            SaveToFile.SaveToExcel(leaguesData, temperature);
        }
    }
}
