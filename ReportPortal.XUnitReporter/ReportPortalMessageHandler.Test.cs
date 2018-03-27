using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        protected virtual void HandleTestStarting(MessageHandlerArgs<ITestStarting> args)
        {
            lock (Logger.LockObject)
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID.ToString();
                Logger.LogMessage($"Starting test: {key} : {testEvent.Test.DisplayName}");

                var tags = new List<string>();
                foreach (var trait in args.Message.Test.TestCase.Traits)
                {
                    foreach (var value in trait.Value)
                    {
                        tags.Add($"{trait.Key}: {value}");
                    }
                }

                TestReporter testReporter = TestReporterDictionary[testEvent.TestCollection.UniqueID.ToString()].StartNewTestNode(
                    new StartTestItemRequest()
                    {
                        Name = testEvent.Test.DisplayName,
                        StartTime = DateTime.UtcNow,
                        Type = TestItemType.Step,
                        Tags = tags
                    });

                TestReporterDictionary[key] = testReporter;
            }
        }

        /// <summary>
        /// Send message about failed test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleFailed(MessageHandlerArgs<ITestFailed> args)
        {
            lock (Logger.LockObject)
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID;

                Logger.LogMessage($"Test failed: {key} : {testEvent.Test.DisplayName}");

                TestReporter testReporter = TestReporterDictionary[key];

                testReporter.Log(new AddLogItemRequest
                {
                    Level = LogLevel.Error,
                    Time = DateTime.UtcNow,
                    Text = $"{ExceptionUtility.CombineMessages(args.Message)}{Environment.NewLine}{ExceptionUtility.CombineStackTraces(args.Message)}"
                });

                if (!string.IsNullOrEmpty(args.Message.Output))
                testReporter.Log(new AddLogItemRequest
                {
                    Level = LogLevel.Debug,
                    Time = DateTime.UtcNow,
                    Text = $"Test output:{Environment.NewLine}{args.Message.Output}"
                });


                testReporter.Finish(new FinishTestItemRequest()
                {
                    EndTime = DateTime.UtcNow,
                    Status = Status.Failed
                });

                TestReporterDictionary.Remove(key);
            }
        }


        /// <summary>
        /// Send message about passed test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandlePassed(MessageHandlerArgs<ITestPassed> args)
        {
            lock (Logger.LockObject)
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID;

                Logger.LogMessage($"Test passed: {key} : {testEvent.Test.DisplayName}");

                TestReporter testReporter = TestReporterDictionary[key];

                if (!string.IsNullOrEmpty(args.Message.Output))
                    testReporter.Log(new AddLogItemRequest
                    {
                        Level = LogLevel.Debug,
                        Time = DateTime.UtcNow,
                        Text = $"Test output:{Environment.NewLine}{args.Message.Output}"
                    });

                testReporter.Finish(new FinishTestItemRequest()
                {
                    EndTime = DateTime.UtcNow,
                    Status = Status.Passed
                });

                TestReporterDictionary.Remove(key);
            }
        }

        /// <summary>
        /// Send message about skipped test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleSkipped(MessageHandlerArgs<ITestSkipped> args)
        {
            lock (Logger.LockObject)
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID;

                Logger.LogMessage($"Test skipped: {key} : {testEvent.Test.DisplayName}");

                TestReporter testReporter = TestReporterDictionary[key];

                testReporter.Finish(new FinishTestItemRequest()
                {
                    EndTime = DateTime.UtcNow,
                    Status = Status.Skipped
                });

                TestReporterDictionary.Remove(key);
            }
        }
    }
}