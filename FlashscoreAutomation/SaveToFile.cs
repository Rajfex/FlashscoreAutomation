using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace FlashscoreAutomation
{
    public class SaveToFile
    {
        public static void SaveToExcel(
            Dictionary<string, List<(string TeamName, int MP, int W, int D, int L, string Goals, int RB, int Pts)>> leaguesData,
            List<(string Country, double Temperature)> temperatures)
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
                        workSheet.Cells[row, 2].Value = team.MP;
                        workSheet.Cells[row, 3].Value = team.W;
                        workSheet.Cells[row, 4].Value = team.D;
                        workSheet.Cells[row, 5].Value = team.L;
                        workSheet.Cells[row, 6].Value = team.Goals;
                        workSheet.Cells[row, 7].Value = team.RB;
                        workSheet.Cells[row, 8].Value = team.Pts;
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

            Logger.Log($"Data saved to {fullPath}");
        }
    }
}