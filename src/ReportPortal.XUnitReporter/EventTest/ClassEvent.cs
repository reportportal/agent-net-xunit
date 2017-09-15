using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region TestClassEvent

        private void HandleClassStartingEvent(MessageHandlerArgs<ITestClassStarting> args)
        {
        }

        private void HandleClassFinishedEvent(MessageHandlerArgs<ITestClassFinished> args)
        {
        }

        private void Execution_TestClassConstructionFinishedEvent(MessageHandlerArgs<ITestClassConstructionFinished> args)
        {
        }

        private void HandleClassConstructionStartingEvent(MessageHandlerArgs<ITestClassConstructionStarting> args)
        {
        }

        private void Execution_TestClassDisposeStartingEvent(MessageHandlerArgs<ITestClassDisposeStarting> args)
        {
        }

        private void Execution_TestClassDisposeFinishedEvent(MessageHandlerArgs<ITestClassDisposeFinished> args)
        {
        }

        private void HandleClassCleanupFailureEvent(MessageHandlerArgs<ITestClassCleanupFailure> args)
        {
        }

        #endregion
    }
}
