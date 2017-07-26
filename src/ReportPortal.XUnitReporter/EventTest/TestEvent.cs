using ReportPortal.Client.Requests;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region TestEvent
        /// <summary>
        /// Send message about failed test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleFailed(MessageHandlerArgs<ITestFailed> args)
        {
            var message = args.Message;
            string key = $"{message.TestCollection.UniqueID}-{message.TestMethod.Method.Name}";
            string messageInLog = $"Test Failed name='{Escape(displayNameFormatter.DisplayName(message.Test))}'\r\n details='{Escape(ExceptionUtility.CombineMessages(message))}\r\n{Escape(ExceptionUtility.CombineStackTraces(message))}' flowId='{ToFlowId(message.TestCollection.DisplayName)}'";

            TestReporterDictionary[key].Log(new AddLogItemRequest()
            {
                Text = ErrorTestMessage(messageInLog, message) + FinishTestMessage(message),
                Level = Client.Models.LogLevel.Error,
                Time = DateTime.UtcNow,
                TestItemId = key
            });
        }


        /// <summary>
        /// Send message about passed test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandlePassed(MessageHandlerArgs<ITestPassed> args)
        {
            var testPassed = args.Message;
            string key = $"{args.Message.TestCollection.UniqueID}-{testPassed.TestMethod.Method.Name}";

            TestReporterDictionary[key].Log(new AddLogItemRequest()
            {
                Text = FinishTestMessage(testPassed),
                Level = Client.Models.LogLevel.Info,
                Time = DateTime.UtcNow,
                TestItemId = key
            });
        }

        /// <summary>
        /// Send message about skipped test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleSkipped(MessageHandlerArgs<ITestSkipped> args)
        {
            var testSkipped = args.Message;
            string key = $"{args.Message.TestCollection.UniqueID}-{testSkipped.TestMethod.Method.Name}";
            string message = $"Test Ignored name='{Escape(displayNameFormatter.DisplayName(testSkipped.Test))}' message='{Escape(testSkipped.Reason)}' flowId='{ToFlowId(testSkipped.TestCollection.DisplayName)}'";

            TestReporterDictionary[key].Log(new AddLogItemRequest()
            {
                Text = message,
                Level = Client.Models.LogLevel.Error,
                Time = DateTime.UtcNow,
                TestItemId = key
            });
        }

        protected virtual void HandleStarting(MessageHandlerArgs<ITestStarting> args)
        {
        }

        private void HandleFinishedEvent(MessageHandlerArgs<ITestFinished> args)
        {
        }

        private void HandleOutputEvent(MessageHandlerArgs<ITestOutput> args)
        {
        }


        protected virtual void HandleCleanupFailure(MessageHandlerArgs<ITestCleanupFailure> args)
        {
        }


        

        #endregion
    }
}
