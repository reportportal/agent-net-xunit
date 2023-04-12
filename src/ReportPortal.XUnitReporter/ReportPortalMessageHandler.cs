using ReportPortal.Client.Abstractions;
using ReportPortal.Shared.Configuration;
using ReportPortal.Shared.Reporter;
using System.Collections.Concurrent;
using Xunit;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler : DefaultRunnerReporterWithTypesMessageHandler
    {
        private readonly IConfiguration _config;

        private readonly IClientService _service;

        private ILaunchReporter _launchReporter;

        protected ConcurrentDictionary<string, ITestReporter> TestReporterDictionary = new ConcurrentDictionary<string, ITestReporter>();

        public ReportPortalReporterMessageHandler(IRunnerLogger logger, IConfiguration configuration) : base(logger)
        {
            _config = configuration;

            _service = new Shared.Reporter.Http.ClientServiceBuilder(configuration).Build();

            Execution.TestAssemblyStartingEvent += TestAssemblyExecutionStarting;
            Execution.TestAssemblyFinishedEvent += TestAssemblyExecutionFinished;

            Execution.TestCollectionStartingEvent += HandleTestCollectionStarting;
            Execution.TestCollectionFinishedEvent += HandleTestCollectionFinished;

            Execution.TestStartingEvent += HandleTestStarting;
            Execution.TestPassedEvent += HandlePassed;
            Execution.TestSkippedEvent += HandleSkipped;
            Execution.TestFailedEvent += HandleFailed;

            Execution.TestOutputEvent += Execution_TestOutputEvent;
        }
    }
}
