using System;
using System.Collections.Generic;
using System.Text;

namespace FlashscoreAutomation.Models
{
    public class LeagueResult
    {
        public string LeagueName { get; set; }

        public List<TeamInfo> Teams { get; set; } = new();
    }
}
