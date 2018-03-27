using ReportPortal.Client;
using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using ReportPortal.XUnitReporter.Configuration;
using System.Collections.Concurrent;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler : TestMessageSink
    {
        private IRunnerLogger Logger { get; set; }

        public static Config Config { get; private set; }

        protected Dictionary<string,TestReporter> TestReporterDictionary = new Dictionary<string, TestReporter>();

        public ReportPortalReporterMessageHandler(IRunnerLogger logger)
        {
            Logger = logger;

            var configPath = Path.GetDirectoryName(new Uri(typeof(Config).Assembly.CodeBase).LocalPath) + "/ReportPortal.config";
            Config = Client.Converters.ModelSerializer.Deserialize<Config>(File.ReadAllText(configPath));

            Logger.LogMessage($".Bridge: {Bridge.Context.GetHashCode()}");

            Bridge.Service = new Service(Config.Server.Url, Config.Server.Project, Config.Server.Authentication.Uuid);

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
