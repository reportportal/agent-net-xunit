using ReportPortal.Client;
using ReportPortal.Client.Abstractions;
using ReportPortal.Shared.Configuration;
using ReportPortal.Shared.Reporter;
using System;
using System.Collections.Generic;
using Xunit;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler : DefaultRunnerReporterWithTypesMessageHandler
    {
        private IConfiguration _config;

        private IClientService _service;

        private ILaunchReporter _launchReporter;

        protected Dictionary<string, ITestReporter> TestReporterDictionary = new Dictionary<string, ITestReporter>();

        public ReportPortalReporterMessageHandler(IRunnerLogger logger, IConfiguration configuration) : base(logger)
        {
            _config = configuration;

            _service = new Service(new Uri(_config.GetValue<string>(ConfigurationPath.ServerUrl)), _config.GetValue<string>(ConfigurationPath.ServerProject), _config.GetValue<string>(ConfigurationPath.ServerAuthenticationUuid));

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
