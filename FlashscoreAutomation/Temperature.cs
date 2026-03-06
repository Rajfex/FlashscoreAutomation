using System;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl.Http;
using Polly;

namespace FlashscoreAutomation
{
    public class Temperature
    {
        public static async Task<double> GetTemperatureAsync(double latitude, double longitude)
        {
            var retryPolicy = Policy<double>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after {timeSpan.Seconds}");
                    });

            try
            {
                double temperature = await retryPolicy.ExecuteAsync(async () =>
                {
                    string url = $"https://api.open-meteo.com/v1/forecast" +
                                 $"?latitude={latitude}&longitude={longitude}" +
                                 $"&current_weather=true&forecast_days=1";

                    var response = await url.GetStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(response);
                    double temp = doc.RootElement[0]
                                     .GetProperty("current_weather")
                                     .GetProperty("temperature")
                                     .GetDouble();

                    return temp;
                });
                Logger.Log("Temperature retrieved successfully");
                return temperature;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to retrieve temperature: {ex.Message}");
                return 0.0;
            }
        }
    }
}