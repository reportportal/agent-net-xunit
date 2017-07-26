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

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler : TestMessageSink
    {
        readonly ReportPortalDisplayNameFormatter displayNameFormatter;
        readonly Func<string, string> flowIdMapper;
        readonly Dictionary<string, string> flowMappings = new Dictionary<string, string>();
        readonly ReaderWriterLockSlim flowMappingsLock = new ReaderWriterLockSlim();
        readonly IRunnerLogger logger;
        string path = $@"{Directory.GetCurrentDirectory()}\rp.porta.epam.txt";
        public static ReportPortalSection Config { get; private set; }
        

        private static Dictionary<string,TestReporter> TestReporterDictionary = new Dictionary<string, TestReporter>();


        public ReportPortalReporterMessageHandler(IRunnerLogger logger,
                                              Func<string, string> flowIdMapper = null,
                                              ReportPortalDisplayNameFormatter displayNameFormatter = null)
        {
            this.logger = logger;
            this.flowIdMapper = flowIdMapper ?? (_ => Guid.NewGuid().ToString("N"));
            this.displayNameFormatter = displayNameFormatter ?? new ReportPortalDisplayNameFormatter();

            Config = Configuration.ReportPortal;
            WebProxy webProxy = Config.Server.Proxy.ElementInformation.IsPresent ? new WebProxy(Config.Server.Proxy.Server) : null;
            Bridge.Service = new Service(new Uri(Config.Server.Url), Config.Server.Project, Config.Server.Authentication.Password, webProxy);

            Execution.TestAssemblyStartingEvent += HandleAssemblyStartingEvent;
            Execution.TestAssemblyFinishedEvent += HandleAssemblyFinishedEvent;

            Execution.TestCollectionStartingEvent += HandleTestCollectionStarting;
            Execution.TestCollectionFinishedEvent += HandleTestCollectionFinished;

            Execution.TestMethodStartingEvent += HandleMethodStartingEvent;
            Execution.TestMethodFinishedEvent += HandleMethodFinishedEvent;

            Execution.TestStartingEvent += HandleStarting;

            Execution.TestPassedEvent += HandlePassed;
            Execution.TestSkippedEvent += HandleSkipped;
            Execution.TestFailedEvent += HandleFailed;

            Execution.TestFinishedEvent += HandleFinishedEvent;
        }
    }
}
