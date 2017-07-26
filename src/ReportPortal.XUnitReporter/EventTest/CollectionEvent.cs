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
        #region TestCollectionEvent

        /// <summary>
        /// Starting test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleTestCollectionStarting(MessageHandlerArgs<ITestCollectionStarting> args)
        {
            var testCollectionStarting = args.Message;
            string key = $"{args.Message.TestCollection.UniqueID}";

            TestReporter testReporter = Bridge.Context.LaunchReporter.StartNewTestNode(
                new StartTestItemRequest()
                {
                    Name = testCollectionStarting.TestCollection.DisplayName,
                    StartTime = DateTime.UtcNow,
                    Type = TestItemType.Suite
                });

            TestReporterDictionary.Add(key, testReporter);
        }

        /// <summary>
        /// Finishing test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleTestCollectionFinished(MessageHandlerArgs<ITestCollectionFinished> args)
        {
            var testCollectionFinished = args.Message;
            string key = $"{args.Message.TestCollection.UniqueID}";

            TestReporterDictionary[key].Finish(new FinishTestItemRequest()
            {
                EndTime = DateTime.UtcNow,
                Status = testCollectionFinished.TestsFailed > 0 ? Status.Failed : Status.Passed
            });
        }

        protected virtual void HandleTestCollectionCleanupFailure(MessageHandlerArgs<ITestCollectionCleanupFailure> args)
        {
        }
        #endregion
    }
}
