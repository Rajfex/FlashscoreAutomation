using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlashscoreAutomation.Logger;
using FlashscoreAutomation.Models;

namespace FlashscoreAutomation.JSONReader
{
    public class ReaderFromJSONService : IReaderFromJSON
    {
        private readonly ILogger _logger;

        public ReaderFromJSONService(ILogger logger)
        {
            _logger = logger;
        }

        public List<FootballLeagueInfo> ReadLeagueInfo()
        {
            string jsonString = File.ReadAllText("leagues.json");
            var root = System.Text.Json.Nodes.JsonNode.Parse(jsonString);
            var leaguesArray = root["FootballLeagueInfo"];
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            _logger.Log("League information read from JSON successfully");

            return leaguesArray.Deserialize<List<FootballLeagueInfo>>(options);
        }
    }
}
