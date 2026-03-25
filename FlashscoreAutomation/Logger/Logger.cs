using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;

namespace FlashscoreAutomation.Logger
{
    public class LoggerSerivce : ILogger
    {
        public LoggerSerivce() 
        {
        }

        public async Task Log(string text)
        {
            using var connection = new SqliteConnection("Data Source=logs.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS Logs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT NOT NULL,
                    Message TEXT NOT NULL
                );
            """;

            command.Parameters.AddWithValue("@timestamp", DateTime.UtcNow);
            command.Parameters.AddWithValue("@message", text);

            await command.ExecuteNonQueryAsync();

            string logPath = "log.txt";
            string logText = $"[{DateTime.UtcNow}]: {text}{Environment.NewLine}";

            await File.AppendAllTextAsync(logPath, logText);

            Console.WriteLine(logText);
        }
    }

}
