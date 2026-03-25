using FlashscoreAutomation.Models;
using FlashscoreAutomation.Logger;
using Microsoft.Playwright;

namespace FlashscoreAutomation.Automations
{
    public class AutomationsService : IAutomations
    {
        private readonly ILogger _logger;

        public AutomationsService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<List<LeagueResult>> GetLeaguesInfoAsync(List<FootballLeagueInfo> leagueInfos)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 250
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            var allLeaguesData = new List<LeagueResult>();

            try
            {
                foreach (var leagueInfo in leagueInfos)
                {
                    string country = leagueInfo.Country.ToLower();
                    string leagueName = leagueInfo.LeaguseName.ToLower().Replace(" ", "-");
                    var url = $"https://www.flashscore.com/football/{country}/{leagueName}/standings/";

                    _logger.Log($"Navigating to {url}");
                    await page.GotoAsync(url);

                    var cookieButton = page.Locator("#onetrust-accept-btn-handler");
                    if (await cookieButton.IsVisibleAsync()) await cookieButton.ClickAsync();

                    await page.WaitForSelectorAsync(".ui-table__row");

                    var rows = page.Locator(".ui-table__row");
                    int rowCount = await rows.CountAsync();

                    var leagueResult = new LeagueResult { LeagueName = leagueInfo.LeaguseName };

                    for (int j = 0; j < rowCount; j++)
                    {
                        var currentRow = rows.Nth(j);
                        var teamName = await currentRow.Locator(".tableCellParticipant__name").InnerTextAsync();
                        var cells = currentRow.Locator(".table__cell--value");

                        var team = new TeamInfo
                        {
                            TeamName = teamName,
                            MatchesPlayed = int.Parse(await cells.Nth(0).InnerTextAsync()),
                            Wins = int.Parse(await cells.Nth(1).InnerTextAsync()),
                            Draws = int.Parse(await cells.Nth(2).InnerTextAsync()),
                            Loosses = int.Parse(await cells.Nth(3).InnerTextAsync()),
                            Goals = await cells.Nth(4).InnerTextAsync(),
                            GoalDifference = int.Parse(await cells.Nth(5).InnerTextAsync()),
                            Points = int.Parse(await cells.Nth(6).InnerTextAsync())
                        };

                        leagueResult.Teams.Add(team);
                    }

                    allLeaguesData.Add(leagueResult);
                }

                _logger.Log("Finished scanning Flashscore");
                return allLeaguesData;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error in scanning Flashscore: {ex.Message}");
                return new List<LeagueResult>();
            }
            finally
            {
                _logger.Log("Closing browser");
                await browser.CloseAsync();
            }
        }
    }
}