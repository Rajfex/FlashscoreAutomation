using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlashscoreAutomation.Models;

namespace FlashscoreAutomation
{
    public class ReadFromJSON
    {
        public static List<FootballLeagueInfo> ReadLeagueInfo()
        {
            string jsonString = File.ReadAllText("leagues.json");
            var root = System.Text.Json.Nodes.JsonNode.Parse(jsonString);
            var leaguesArray = root["FootballLeagueInfo"];
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            Logger.Log("League information read from JSON successfully");

            return leaguesArray.Deserialize<List<FootballLeagueInfo>>(options);
        }
    }
}
