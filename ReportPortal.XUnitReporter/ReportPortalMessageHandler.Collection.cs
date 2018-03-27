using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        /// <summary>
        /// Starting test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleTestCollectionStarting(MessageHandlerArgs<ITestCollectionStarting> args)
        {
            lock (Logger.LockObject)
            {
                var testCollection = args.Message;
                string key = testCollection.TestCollection.UniqueID.ToString();
                Logger.LogMessage($"Starting test collection: {key} : {testCollection.TestCollection.DisplayName}");

                TestReporter testReporter = Bridge.Context.LaunchReporter.StartNewTestNode(
                    new StartTestItemRequest()
                    {
                        Name = testCollection.TestCollection.DisplayName,
                        StartTime = DateTime.UtcNow,
                        Type = TestItemType.Suite
                    });

                TestReporterDictionary[key] = testReporter;
            }
        }

        /// <summary>
        /// Finishing test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleTestCollectionFinished(MessageHandlerArgs<ITestCollectionFinished> args)
        {
            lock (Logger.LockObject)
            {
                var testCollection = args.Message;
                string key = testCollection.TestCollection.UniqueID.ToString();
                Logger.LogMessage($"Finishing test collection: {key} : {testCollection.TestCollection.DisplayName}");

                TestReporterDictionary[key].Finish(new FinishTestItemRequest()
                {
                    EndTime = DateTime.UtcNow,
                    Status = testCollection.TestsFailed > 0 ? Status.Failed : Status.Passed
                });
            }
        }
    }
}
