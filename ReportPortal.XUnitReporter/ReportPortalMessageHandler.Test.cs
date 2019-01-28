using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared.Reporter;
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
                try
                {
                    var testEvent = args.Message;
                    string key = testEvent.Test.TestCase.UniqueID.ToString();

                    var tags = new List<string>();
                    foreach (var trait in args.Message.Test.TestCase.Traits)
                    {
                        foreach (var value in trait.Value)
                        {
                            tags.Add($"{trait.Key}: {value}");
                        }
                    }

                    ITestReporter testReporter = TestReporterDictionary[testEvent.TestCollection.UniqueID.ToString()].StartChildTestReporter(
                        new StartTestItemRequest()
                        {
                            Name = testEvent.Test.DisplayName,
                            StartTime = DateTime.UtcNow,
                            Type = TestItemType.Step,
                            Tags = tags
                        });

                    TestReporterDictionary[key] = testReporter;
                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
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
                try
                {
                    var testEvent = args.Message;
                    string key = testEvent.Test.TestCase.UniqueID;

                    ITestReporter testReporter = TestReporterDictionary[key];

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
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
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
                try
                {
                    var testEvent = args.Message;
                    string key = testEvent.Test.TestCase.UniqueID;

                    ITestReporter testReporter = TestReporterDictionary[key];

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
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
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
                try
                {
                    var testEvent = args.Message;
                    string key = testEvent.Test.TestCase.UniqueID;

                    ITestReporter testReporter = TestReporterDictionary[key];

                    testReporter.Finish(new FinishTestItemRequest()
                    {
                        EndTime = DateTime.UtcNow,
                        Status = Status.Skipped,
                        Issue = new Issue
                        {
                            Type = WellKnownIssueType.NotDefect,
                            Comment = testEvent.Reason
                        }
                    });

                    TestReporterDictionary.Remove(key);
                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
            }
        }

        protected virtual void Execution_TestOutputEvent(MessageHandlerArgs<ITestOutput> args)
        {
            lock (Logger.LockObject)
            {
                try
                {
                    var testEvent = args.Message;
                    string key = testEvent.Test.TestCase.UniqueID;

                    ITestReporter testReporter = TestReporterDictionary[key];

                    testReporter.Log(new AddLogItemRequest
                    {
                        Level = LogLevel.Info,
                        Time = DateTime.UtcNow,
                        Text = testEvent.Output
                    });
                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                }
            }
        }
    }
}