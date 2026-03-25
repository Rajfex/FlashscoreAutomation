using System;
using System.Collections.Generic;
using System.Text;

namespace FlashscoreAutomation.TemperatureReader
{
    internal interface ITemperature
    {
        Task<double> GetTemperatureAsync(double latitude, double longitude);
    }
}
