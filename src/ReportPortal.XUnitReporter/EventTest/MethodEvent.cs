using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;
using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region TestMethodEvent

        /// <summary>
        /// Starting test method in report portal.
        /// </summary>
        /// <param name="args"></param>
        private void HandleMethodStartingEvent(MessageHandlerArgs<ITestMethodStarting> args)
        {
            var methodStartingMassage = args.Message;
            string key = $"{args.Message.TestCollection.UniqueID}-{methodStartingMassage.TestMethod.Method.Name}";

            TestReporter testReporter = TestReporterDictionary[$"{args.Message.TestCollection.UniqueID}"].StartNewTestNode(
                new StartTestItemRequest()
                {
                    Name = methodStartingMassage.TestMethod.Method.Name,
                    StartTime = DateTime.UtcNow,
                    Type = TestItemType.Step
                });

            TestReporterDictionary.Add(key, testReporter);
        }

        /// <summary>
        /// Finishing test method in report portal.
        /// </summary>
        /// <param name="args"></param>
        private void HandleMethodFinishedEvent(MessageHandlerArgs<ITestMethodFinished> args)
        {
            var methodFinishedMassage = args.Message;
            string key = $"{args.Message.TestCollection.UniqueID}-{methodFinishedMassage.TestMethod.Method.Name}";
            
            TestReporterDictionary[key].Finish(new FinishTestItemRequest()
            {
                EndTime = DateTime.UtcNow,
                Status = methodFinishedMassage.TestsFailed > 0 ? Status.Failed : Status.Passed
            });

        }

        protected virtual void HandleTestMethodCleanupFailure(MessageHandlerArgs<ITestMethodCleanupFailure> args)
        {
        }

        #endregion

    }
}
