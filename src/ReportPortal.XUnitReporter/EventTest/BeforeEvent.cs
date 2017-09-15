using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region BeforeTestEvent
        private void Execution_BeforeTestFinishedEvent(MessageHandlerArgs<IBeforeTestFinished> args)
        {
        }

        private void Execution_BeforeTestStartingEvent(MessageHandlerArgs<IBeforeTestStarting> args)
        {
        }

        #endregion
    }
}
