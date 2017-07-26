using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region TestCaseEvent

        private void Execution_TestCaseStartingEvent(MessageHandlerArgs<ITestCaseStarting> args)
        {
        }

        private void Execution_TestCaseFinishedEvent(MessageHandlerArgs<ITestCaseFinished> args)
        {
        }

        protected virtual void HandleTestCaseCleanupFailure(MessageHandlerArgs<ITestCaseCleanupFailure> args)
        {
        }

        protected virtual void HandleTestCaseCleanupFailure(MessageHandlerArgs<ITestClassCleanupFailure> args)
        {
        }

        #endregion
    }
}
