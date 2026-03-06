using System;
using System.Collections.Generic;
using System.Text;

namespace FlashscoreAutomation.Models
{
    public class LeagueResult
    {
        public string LeagueName { get; set; }
        public List<TeamStanding> Teams { get; set; } = new();
    }
}
