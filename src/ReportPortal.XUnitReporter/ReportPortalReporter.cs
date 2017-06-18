using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public class ReportPortalReporter : IRunnerReporter
    {
        public string Description => "Reporting tests results to Report Portal";

        public bool IsEnvironmentallyEnabled => true;

        public string RunnerSwitch => "reportportal";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger) => new ReportPortalMessageHandler(logger);
    }
}
