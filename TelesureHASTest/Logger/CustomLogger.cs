using System;
using System.Collections.Generic;
using System.Text;

namespace TelesureHASTest.Logger
{
    public class CustomLogger : ICustomLogger
    {
        private static void Log(string tag, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{tag}: {message}");
        }

        public void Error(string tag, string message)
        {
            Log(tag, message);
        }

        public void Info(string tag, string message)
        {
            Log(tag, message);
        }

        public void Debug(string tag, string message)
        {
            Log(tag, message);
        }
    }
}
