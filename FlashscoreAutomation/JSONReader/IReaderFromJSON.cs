using System;
using System.Collections.Generic;
using System.Text;
using FlashscoreAutomation.Models;

namespace FlashscoreAutomation.JSONReader
{
    internal interface IReaderFromJSON
    {
        List<FootballLeagueInfo> ReadLeagueInfo();
    }
}
