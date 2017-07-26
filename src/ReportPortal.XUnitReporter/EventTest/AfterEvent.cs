using ReportPortal.Client.Requests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region AfterTestEvent

        private void Execution_AfterTestStartingEvent(MessageHandlerArgs<IAfterTestStarting> args)
        {
            LogImportant($"Execution_AfterTestStartingEvent");
        }

        private void Execution_AfterTestFinishedEvent(MessageHandlerArgs<IAfterTestFinished> args)
        {
            LogImportant($"Execution_AfterTestFinishedEvent");
        }

        #endregion
    }
}
