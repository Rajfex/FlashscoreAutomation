using FlashscoreAutomation.Models;

namespace FlashscoreAutomation.FileWriter
{
    public interface IFileWriter
    {
        void SaveToExcel(Dictionary<string, List<TeamInfo>> leaguesData, List<(string Country, double Temperature)> temperatures);
    }
}
