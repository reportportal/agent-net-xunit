using Xunit.Abstractions;

namespace Xunit
{
    public static class OutputHelperExtensions
    {
        public static ITestOutputHelper WithReportPortal(this ITestOutputHelper outputHelper)
        {
            ReportPortal.XUnitReporter.LogHandler.LogHandler.XunitTestOutputHelper = outputHelper;

            return outputHelper;
        }
    }
}
