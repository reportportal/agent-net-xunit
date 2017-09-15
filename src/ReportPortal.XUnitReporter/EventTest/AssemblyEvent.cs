using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region TestAssemblyEvent

        /// <summary>
        /// Starting connect to report portal. Create launcher and start it.
        /// </summary>
        /// <param name="args"></param>
        private void HandleAssemblyStartingEvent(MessageHandlerArgs<ITestAssemblyStarting> args)
        {
            LaunchMode launchMode = Config.Launch.DebugMode ? LaunchMode.Debug : LaunchMode.Default;

            StartLaunchRequest startLaunchRequest = new StartLaunchRequest
            {
                Name = Config.Launch.Name,
                StartTime = DateTime.UtcNow,
                Mode = launchMode,
                Tags = Config.Launch.Tags.Split(',').ToList(),
                Description = "Description"
            };
            
            Bridge.Context.LaunchReporter = new LaunchReporter(Bridge.Service);
            Bridge.Context.LaunchReporter.Start(startLaunchRequest);
        }

        /// <summary>
        /// Finish connect to report portal. Finish launcher, wait threads and close connect.
        /// </summary>
        /// <param name="args"></param>
        private void HandleAssemblyFinishedEvent(MessageHandlerArgs<ITestAssemblyFinished> args)
        {
            Bridge.Context.LaunchReporter.Finish(new FinishLaunchRequest { EndTime = DateTime.UtcNow });
            Bridge.Context.LaunchReporter.FinishTask.Wait();
        }

        protected virtual void HandleTestAssemblyCleanupFailure(MessageHandlerArgs<ITestAssemblyCleanupFailure> args)
        {
        }

        #endregion
    }
}
