using System;
using System.Collections.Generic;
using System.Text;

namespace TelesureHASTest.Logger
{
    public interface ICustomLogger
    {
        public void Error(string tag, string message);
        public void Info(string tag, string message);
        public void Debug(string tag, string message);
    }
}
