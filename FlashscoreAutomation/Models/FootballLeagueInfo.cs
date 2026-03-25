using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FlashscoreAutomation.Models
{
    public class FootballLeagueInfo
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("leagueName")]
        public string LeaguseName { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
