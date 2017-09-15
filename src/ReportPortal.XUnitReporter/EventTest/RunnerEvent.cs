using Xunit;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region Runner

        private void Runner_TestExecutionSummaryEvent(MessageHandlerArgs<ITestExecutionSummary> args)
        {
        }

        private void Runner_TestAssemblyExecutionStartingEvent(MessageHandlerArgs<ITestAssemblyExecutionStarting> args)
        {
        }

        private void Runner_TestAssemblyExecutionFinishedEvent(MessageHandlerArgs<ITestAssemblyExecutionFinished> args)
        {
        }

        private void Runner_TestAssemblyDiscoveryStartingEvent(MessageHandlerArgs<ITestAssemblyDiscoveryStarting> args)
        {
        }

        private void Runner_TestAssemblyDiscoveryFinishedEvent(MessageHandlerArgs<ITestAssemblyDiscoveryFinished> args)
        {
        }

        #endregion
    }
}
