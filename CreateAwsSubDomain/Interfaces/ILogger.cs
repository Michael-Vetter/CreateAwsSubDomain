using System;
using System.Collections.Generic;
using System.Text;

namespace CreateAwsSubDomain.Interfaces
{
    public interface ILogger
    {
        void LogInformation(string data);
        void LogError(string data);
    }
}
