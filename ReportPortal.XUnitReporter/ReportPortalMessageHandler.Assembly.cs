using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;
using System;
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
                LaunchMode launchMode = Config.Launch.IsDebugMode ? LaunchMode.Debug : LaunchMode.Default;

                StartLaunchRequest startLaunchRequest = new StartLaunchRequest
                {
                    Name = Config.Launch.Name,
                    StartTime = DateTime.UtcNow,
                    Mode = launchMode,
                    Tags = Config.Launch.Tags,
                    Description = $"{Config.Launch.Description}{Environment.NewLine}Environment: {args.Message.TestEnvironment}, Platform: {args.Message.TestFrameworkDisplayName}"
                };

                Bridge.Context.LaunchReporter = new LaunchReporter(Bridge.Service);
                Bridge.Context.LaunchReporter.Start(startLaunchRequest);
            }
        }

        protected virtual void TestAssemblyExecutionFinished(MessageHandlerArgs<ITestAssemblyFinished> args)
        {
            lock (Logger.LockObject)
            {
                Logger.LogMessage(".AssemblyFinishedEvent");
                Bridge.Context.LaunchReporter.Finish(new FinishLaunchRequest { EndTime = DateTime.UtcNow });

                var stopWatch = Stopwatch.StartNew();
                Logger.LogMessage("Waiting to finish sending results to Report Portal server...");

                Bridge.Context.LaunchReporter.FinishTask.Wait();
                Logger.LogMessage($"Results are sent to Report Portal server. Sync duration: {stopWatch.Elapsed}");
            }
        }
    }
}
