using FlashscoreAutomation.Models;
using FlashscoreAutomation.Logger;
using OfficeOpenXml;

namespace FlashscoreAutomation.FileWriter
{
    public class ExcelFileWriterService : IFileWriter
    {
        private readonly ILogger _logger;

        public ExcelFileWriterService(ILogger logger) 
        {
            _logger = logger;
        }

        public void SaveToExcel(Dictionary<string, List<TeamInfo>> leaguesData, List<(string Country, double Temperature)> temperatures)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = Path.Combine(folder, "FlashscoreStandings.xlsx");
            FileInfo file = new FileInfo(fullPath);

            if (file.Exists)
                file.Delete();

            using (var package = new ExcelPackage(file))
            {
                var workSheet = package.Workbook.Worksheets.Add("Leagues");
                int row = 1;

                foreach (var league in leaguesData)
                {
                    workSheet.Cells[row, 1].Value = league.Key;
                    row++;

                    workSheet.Cells[row, 1].Value = "Team";
                    workSheet.Cells[row, 2].Value = "MP";
                    workSheet.Cells[row, 3].Value = "W";
                    workSheet.Cells[row, 4].Value = "D";
                    workSheet.Cells[row, 5].Value = "L";
                    workSheet.Cells[row, 6].Value = "Goals";
                    workSheet.Cells[row, 7].Value = "RB";
                    workSheet.Cells[row, 8].Value = "Pts";
                    row++;

                    foreach (var team in league.Value)
                    {
                        workSheet.Cells[row, 1].Value = team.TeamName;
                        workSheet.Cells[row, 2].Value = team.MatchesPlayed;
                        workSheet.Cells[row, 3].Value = team.Wins;
                        workSheet.Cells[row, 4].Value = team.Draws;
                        workSheet.Cells[row, 5].Value = team.Loosses;
                        workSheet.Cells[row, 6].Value = team.Goals;
                        workSheet.Cells[row, 7].Value = team.GoalDifference;
                        workSheet.Cells[row, 8].Value = team.Points;
                        row++;
                    }

                    row += 2;
                }

                if (temperatures != null && temperatures.Count > 0)
                {
                    workSheet.Cells[row, 1].Value = "Temperatures";
                    row++;

                    workSheet.Cells[row, 1].Value = "Country";
                    workSheet.Cells[row, 2].Value = "Temperature";
                    row++;

                    foreach (var temp in temperatures)
                    {
                        workSheet.Cells[row, 1].Value = temp.Country;
                        workSheet.Cells[row, 2].Value = temp.Temperature;
                        row++;
                    }
                }

                package.Save();
            }

            _logger.Log($"Data saved to {fullPath}");
        }
    }
}