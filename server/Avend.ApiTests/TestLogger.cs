using System.IO;

using Avend.ApiTests.Infrastructure;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serilog;

namespace Avend.ApiTests
{
    [TestClass]
    public class TestLogger
    {
        [TestMethod]
        public void TestLoggerMethod()
        {
            TestSystem.FixWorkingDirectory();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Directory.GetCurrentDirectory() + "\\appsettings.json")                
                .Build();

            Log.Logger = new LoggerConfiguration()                
                .ReadFrom.Configuration(configuration)                                
                .Enrich.FromLogContext()
                .CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilogProvider();

            var msLogger = loggerFactory.CreateLogger<TestLogger>();
            msLogger.LogDebug(LoggingEvents.STARTUP, "HELLO MS {0:l} {1}", "TEST", "ZERO");
            using (msLogger.BeginScope("NEW SCOPE"))
            {
                msLogger.LogInformation(LoggingEvents.STARTUP, "HELLO MS Scope - {Test2} {Zero} INFO", "TEST2", "ZERO");
                using (msLogger.BeginScope("NESTED Scope"))
                {
                    msLogger.LogError("ARG1 = {Arg1} Arg2 = {Arg2}", "ARG1", "Arg2");
                }
                msLogger.LogInformation("Scope again");
            }
            msLogger.LogWarning("No scope again");
        }
    }
}