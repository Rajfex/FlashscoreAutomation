using System;
using System.Collections.Generic;
using System.Text;

namespace FlashscoreAutomation.Models
{
    public class TeamInfo
    {
        public string TeamName { get; set; }

        public int MatchesPlayed { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Loosses { get; set; }

        public string Goals { get; set; }

        public int GoalDifference { get; set; }

        public int Points { get; set; }
    }
}
