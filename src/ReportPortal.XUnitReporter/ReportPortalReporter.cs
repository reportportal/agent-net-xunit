using ReportPortal.Shared.Configuration;
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
            var currentDirectory = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ReportPortalReporter).Assembly.Location).LocalPath));

            _config = new ConfigurationBuilder().AddDefaults(currentDirectory).Build();
        }

        public string Description => "Reporting tests results to Report Portal";

        public bool IsEnvironmentallyEnabled => _config.GetValue("enabled", true);

        public string RunnerSwitch => "reportportal";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger) => new ReportPortalReporterMessageHandler(logger, _config);
    }
}
