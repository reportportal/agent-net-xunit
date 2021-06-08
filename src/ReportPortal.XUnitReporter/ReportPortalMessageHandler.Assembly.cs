using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Shared.Configuration;
using ReportPortal.Shared.Reporter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        /// <summary>
        /// Starting connect to report portal. Create launcher and start it.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void TestAssemblyExecutionStarting(MessageHandlerArgs<ITestAssemblyStarting> args)
        {
            lock (Logger.LockObject)
            {
                try
                {
                    LaunchMode launchMode = _config.GetValue(ConfigurationPath.LaunchDebugMode, false) ? LaunchMode.Debug : LaunchMode.Default;

                    var startLaunchRequest = new StartLaunchRequest
                    {
                        Name = _config.GetValue(ConfigurationPath.LaunchName, "xUnit Demo Launch"),
                        StartTime = DateTime.UtcNow,
                        Mode = launchMode,
                        Attributes = _config.GetKeyValues("Launch:Attributes", new List<KeyValuePair<string, string>>()).Select(a => new ItemAttribute { Key = a.Key, Value = a.Value }).ToList(),
                        Description = _config.GetValue(ConfigurationPath.LaunchDescription, "")
                    };

                    Shared.Extensibility.Analytics.AnalyticsReportEventsObserver.DefineConsumer("agent-dotnet-xunit");

                    _launchReporter = new LaunchReporter(_service, _config, null, Shared.Extensibility.ExtensionManager.Instance);
                    _launchReporter.Start(startLaunchRequest);
                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
            }
        }

        protected virtual void TestAssemblyExecutionFinished(MessageHandlerArgs<ITestAssemblyFinished> args)
        {
            lock (Logger.LockObject)
            {
                try
                {
                    _launchReporter.Finish(new FinishLaunchRequest { EndTime = DateTime.UtcNow });

                    Logger.LogMessage("Waiting to finish sending results to Report Portal server...");

                    var stopWatch = Stopwatch.StartNew();

                    _launchReporter.Sync();

                    Logger.LogMessage($"Results are sent to Report Portal server. Sync duration: {stopWatch.Elapsed}");
                    Logger.LogMessage(_launchReporter.StatisticsCounter.ToString());
                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
            }
        }
    }
}
