using ReportPortal.Client;
using ReportPortal.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

using ReportPortal.Shared.Reporter;
using ReportPortal.Shared.Configuration;
using ReportPortal.Shared.Configuration.Providers;
using ReportPortal.Client.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler : TestMessageSink
    {
        private IRunnerLogger Logger { get; set; }

        private IConfiguration _config;

        private IClientService _service;

        private ILaunchReporter _launchReporter;

        protected Dictionary<string, ITestReporter> TestReporterDictionary = new Dictionary<string, ITestReporter>();

        public ReportPortalReporterMessageHandler(IRunnerLogger logger)
        {
            Logger = logger;

            var jsonPath = Path.GetDirectoryName(new Uri(typeof(ReportPortalReporter).Assembly.CodeBase).LocalPath) + "/ReportPortal.config.json";

            Logger.LogMessage($"ReportPortal json config: {jsonPath}");

            _config = new ConfigurationBuilder().AddJsonFile(jsonPath).AddEnvironmentVariables().Build();

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
