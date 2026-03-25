using FlashscoreAutomation.Models;
using OfficeOpenXml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FlashscoreAutomation.Automations;
using FlashscoreAutomation.FileWriter;
using FlashscoreAutomation.JSONReader;
using FlashscoreAutomation.Logger;
using FlashscoreAutomation.TemperatureReader;

namespace FlashscoreAutomation
{

    internal class Program
    {
        public static async Task Main(string[] args)
        {

            using var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTransient<IAutomations, AutomationsService>();
                    services.AddTransient<IFileWriter, ExcelFileWriterService>();
                    services.AddTransient<IReaderFromJSON, ReaderFromJSONService>();
                    services.AddTransient<ILogger, LoggerSerivce>();
                    services.AddTransient<ITemperature, TemperatureService>();
                })
                .Build();

            var automations = host.Services.GetRequiredService<IAutomations>();
            var fileWriter = host.Services.GetRequiredService<IFileWriter>();
            var readerFromJSON = host.Services.GetRequiredService<IReaderFromJSON>();
            var logger = host.Services.GetRequiredService<ILogger>();
            var temperature = host.Services.GetRequiredService<ITemperature>();
            var leaguesData = new Dictionary<string, List<TeamInfo>>();

            logger.Log("Application started");

            ExcelPackage.License.SetNonCommercialPersonal("Jakub");
            List<FootballLeagueInfo> leagueInfos = readerFromJSON.ReadLeagueInfo();

            var results = await automations.GetLeaguesInfoAsync(leagueInfos);

            foreach (var league in results)
            {
                var list = new List<TeamInfo>();
                foreach (var team in league.Teams)
                {
                    list.Add(team);
                }
                leaguesData.Add(league.LeagueName, list);
            }

            var tempList = new List<(string Country, double Temperature)>();

            foreach (var temp in leagueInfos)
            {
                var tempData = await temperature.GetTemperatureAsync(temp.Latitude, temp.Longitude);
                tempList.Add((temp.Country, tempData));
            }

            fileWriter.SaveToExcel(leaguesData, tempList);
        }
    }
}
