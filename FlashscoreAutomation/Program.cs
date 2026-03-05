using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Playwright;
using OfficeOpenXml;

namespace FlashscoreAutomation
{

    internal class Program
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
        public static List<FootballLeagueInfo> ReadLeagueInfoJSON()
        {
            string jsonString = File.ReadAllText("leagues.json");

            var root = System.Text.Json.Nodes.JsonNode.Parse(jsonString);

            var leaguesArray = root["FootballLeagueInfo"];

            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return leaguesArray.Deserialize<List<FootballLeagueInfo>>(options);
        }


        public static async Task<double> GetTemperature(double latitude, double longitude)
        {
            using HttpClient client = new HttpClient();
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&current=temperature_2m&forecast_days=1";

            HttpResponseMessage response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseBody);

            var location = doc.RootElement[0];

            double temperature = location
                .GetProperty("current")
                .GetProperty("temperature_2m")
                .GetDouble();

            return temperature;
        }

        public static async Task Main(string[] args)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Jakub");


            List<FootballLeagueInfo> leagueInfos = ReadLeagueInfoJSON();

            for (int i = 0; i < leagueInfos.Count; i++)
            {
                Console.WriteLine($"Country: {leagueInfos[i].Country}");
                Console.WriteLine($"League Name: {leagueInfos[i].LeaguseName}");
                Console.WriteLine($"Latitude: {leagueInfos[i].Latitude}");
                Console.WriteLine($"Longitude: {leagueInfos[i].Longitude}");
                Console.WriteLine();
            }

            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 500
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            var allLeaguesData = new List<(string TeamName, int MP, int W, int D, int L, string Goals, int Pts)>();
            var temperatures = new List<(string Country, double Temperature)>();

            try
            {
                for (int i = 0; i < leagueInfos.Count; i++)
                {
                    Console.WriteLine($"Processing {leagueInfos[i].Latitude} - {leagueInfos[i].Longitude}");
                    string country = leagueInfos[i].Country.ToLower();
                    string leagueName = leagueInfos[i].LeaguseName.ToLower().Replace(" ", "-");
                    var url = $"https://www.flashscore.com/football/{country}/{leagueName}/standings/";

                    await page.GotoAsync(url);

                    var cookieButton = page.Locator("#onetrust-accept-btn-handler");
                    if (await cookieButton.IsVisibleAsync()) await cookieButton.ClickAsync();

                    await page.WaitForSelectorAsync(".ui-table__row");

                    var rows = page.Locator(".ui-table__row");
                    int rowCount = await rows.CountAsync();

                    for (int j = 0; j < rowCount; j++)
                    {
                        var currentRow = rows.Nth(j);

                        var teamName = await currentRow.Locator(".tableCellParticipant__name").InnerTextAsync();

                        var cells = currentRow.Locator(".table__cell--value");

                        allLeaguesData.Add((
                            teamName,
                            Convert.ToInt32(await cells.Nth(0).InnerTextAsync()), // MP 
                            Convert.ToInt32(await cells.Nth(1).InnerTextAsync()), // W 
                            Convert.ToInt32(await cells.Nth(2).InnerTextAsync()), // D 
                            Convert.ToInt32(await cells.Nth(3).InnerTextAsync()), // L 
                            await cells.Nth(4).InnerTextAsync(),                // Goals
                            Convert.ToInt32(await cells.Nth(5).InnerTextAsync())  // Pts
                        ));
                    }

                    double temp = await GetTemperature(leagueInfos[i].Latitude, leagueInfos[i].Longitude);
                    temperatures.Add((leagueInfos[i].Country, temp));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }
            finally
            {
                await browser.CloseAsync();
            }
        }
    }
}
