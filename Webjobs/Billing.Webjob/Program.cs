using System;
using Microsoft.Azure.WebJobs;
using NLog;

namespace Billing.Webjob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Console.Title = "Billing's webjob";

            InitLog();

            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var serviceBusHost = new ServiceBusHost();
            serviceBusHost.StartAsync().ConfigureAwait(false)
                .GetAwaiter().GetResult();

            Logger.Info("Salesforce's webjob is starting");
            var host = new JobHost(config);

            host.RunAndBlock();

            serviceBusHost.StopAsync().GetAwaiter().GetResult();
        }

        private static void InitLog()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget { FileName = "log.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget();

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }
    }
}
