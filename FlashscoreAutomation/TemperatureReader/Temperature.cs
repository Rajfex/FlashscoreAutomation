using System;
using System.Text.Json;
using System.Threading.Tasks;
using FlashscoreAutomation.Logger;
using Flurl.Http;
using Polly;

namespace FlashscoreAutomation.TemperatureReader
{
    public class TemperatureService : ITemperature
    {
        public TemperatureService() 
        {
        }

        private static readonly ILogger LoggerService = new LoggerSerivce();

        public async Task<double> GetTemperatureAsync(double latitude, double longitude)
        {
            var retryPolicy = Policy<double>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after {timeSpan.Seconds}");
                    });

            var result = await retryPolicy.ExecuteAndCaptureAsync(async () =>
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

            if (result.Outcome == OutcomeType.Successful)
            {
                await LoggerService.Log("Temperature retrieved successfully");
                return result.Result;
            }

            await LoggerService.Log($"Failed to retrieve temperature: {result.FinalException?.Message}");
            return 0.0;
        }
    }
}