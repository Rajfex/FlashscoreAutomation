using System;
using System.Collections.Generic;
using System.Text;

namespace FlashscoreAutomation.Logger
{
    public interface ILogger
    {
        Task Log(string text);
    }
}
