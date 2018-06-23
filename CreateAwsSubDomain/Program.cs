using System;
using CreateAwsSubDomain.businessLayer;
using CreateAwsSubDomain.cloudLayer;
using CreateAwsSubDomain.Interfaces;

namespace CreateAwsSubDomain
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ConsoleLogger consoleLogger = new ConsoleLogger();
            
            AwsCommands AwsContext = new AwsCommands(consoleLogger);

            CreateSubDomain creator = new CreateSubDomain(AwsContext, consoleLogger);
            Console.Write("Enter the full name of the subdomain to create, including the parent name: ");

            string input = Console.ReadLine();

            // example input:  ac.serverless.noraininmycloud.com

            if (creator.execute(input))
                consoleLogger.LogInformation("success");
            else
                consoleLogger.LogError("");

            Console.ReadLine();


        }

    }
    class ConsoleLogger : ILogger
    {
        public void LogError(string data)
        {
            Console.WriteLine("error: {0}", data);
        }

        public void LogInformation(string data)
        {
            Console.WriteLine("Information: {0}", data);
        }
    }
}