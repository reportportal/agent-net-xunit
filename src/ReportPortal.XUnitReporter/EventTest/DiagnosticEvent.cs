using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        protected virtual void HandleDiagnosticsErrorMessage(MessageHandlerArgs<IErrorMessage> args)
        {
        }

        private void HandleDiagnosticMessageEvent(MessageHandlerArgs<IDiagnosticMessage> args)
        {
        }
    }
}
