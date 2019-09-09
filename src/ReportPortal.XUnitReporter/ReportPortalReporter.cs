using ReportPortal.Shared.Configuration;
using ReportPortal.Shared.Configuration.Providers;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public class ReportPortalReporter : IRunnerReporter
    {
        private IConfiguration _config;

        public ReportPortalReporter()
        {
            var jsonPath = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ReportPortalReporter).Assembly.CodeBase).LocalPath), "ReportPortal.config.json");
            _config = new ConfigurationBuilder().AddJsonFile(jsonPath).AddEnvironmentVariables().Build();
        }

        public string Description => "Reporting tests results to Report Portal";

        public bool IsEnvironmentallyEnabled => _config.GetValue("enabled", true);

        public string RunnerSwitch => "reportportal";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger) => new ReportPortalReporterMessageHandler(logger);
    }
}
