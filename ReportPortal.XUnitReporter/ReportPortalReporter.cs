using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public class ReportPortalReporter : IRunnerReporter
    {
        public string Description => "Reporting tests results to Report Portal";

        public bool IsEnvironmentallyEnabled => true;

        public string RunnerSwitch => "reportportal";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger) => new ReportPortalReporterMessageHandler(logger);
    }
}
