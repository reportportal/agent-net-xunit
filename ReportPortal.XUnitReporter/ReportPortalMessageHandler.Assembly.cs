using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;
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

                    StartLaunchRequest startLaunchRequest = new StartLaunchRequest
                    {
                        Name = _config.GetValue(ConfigurationPath.LaunchName, "xUnit Demo Launch"),
                        StartTime = DateTime.UtcNow,
                        Mode = launchMode,
                        Tags = _config.GetValues(ConfigurationPath.LaunchTags, new List<string>()).ToList(),
                        Description = _config.GetValue(ConfigurationPath.LaunchDescription, "")
                    };

                    Bridge.Context.LaunchReporter = new LaunchReporter(Bridge.Service);
                    Bridge.Context.LaunchReporter.Start(startLaunchRequest);
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
                    Bridge.Context.LaunchReporter.Finish(new FinishLaunchRequest { EndTime = DateTime.UtcNow });

                    Logger.LogMessage("Waiting to finish sending results to Report Portal server...");

                    var stopWatch = Stopwatch.StartNew();

                    Bridge.Context.LaunchReporter.FinishTask.Wait();

                    Logger.LogMessage($"Results are sent to Report Portal server. Sync duration: {stopWatch.Elapsed}");
                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
            }
        }
    }
}
