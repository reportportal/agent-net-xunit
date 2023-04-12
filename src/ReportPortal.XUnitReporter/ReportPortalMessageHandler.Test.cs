using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Client.Abstractions.Responses;
using ReportPortal.Shared.Reporter;
using ReportPortal.XUnitReporter.LogHandler.Messages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        protected virtual void HandleTestStarting(MessageHandlerArgs<ITestStarting> args)
        {
            try
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID.ToString();

                var attrbutes = new List<ItemAttribute>();
                foreach (var trait in args.Message.Test.TestCase.Traits)
                {
                    foreach (var value in trait.Value)
                    {
                        attrbutes.Add(new ItemAttribute { Key = trait.Key, Value = value });
                    }
                }

                ITestReporter testReporter = TestReporterDictionary[testEvent.TestCollection.UniqueID.ToString()].StartChildTestReporter(
                    new StartTestItemRequest()
                    {
                        Name = testEvent.Test.DisplayName,
                        StartTime = DateTime.UtcNow,
                        Type = TestItemType.Step,
                        Attributes = attrbutes
                    });

                TestReporterDictionary[key] = testReporter;
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }

        /// <summary>
        /// Send message about failed test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleFailed(MessageHandlerArgs<ITestFailed> args)
        {
            try
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID;

                ITestReporter testReporter = TestReporterDictionary[key];

                testReporter.Log(new CreateLogItemRequest
                {
                    Level = LogLevel.Error,
                    Time = DateTime.UtcNow,
                    Text = $"{ExceptionUtility.CombineMessages(args.Message)}{Environment.NewLine}{ExceptionUtility.CombineStackTraces(args.Message)}"
                });

                if (!string.IsNullOrEmpty(args.Message.Output))
                    testReporter.Log(new CreateLogItemRequest
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

                TestReporterDictionary.TryRemove(key, out _);
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }


        /// <summary>
        /// Send message about passed test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandlePassed(MessageHandlerArgs<ITestPassed> args)
        {
            try
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID;

                ITestReporter testReporter = TestReporterDictionary[key];

                if (!string.IsNullOrEmpty(args.Message.Output))
                    testReporter.Log(new CreateLogItemRequest
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

                TestReporterDictionary.TryRemove(key, out _);
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }

        /// <summary>
        /// Send message about skipped test in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleSkipped(MessageHandlerArgs<ITestSkipped> args)
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

                TestReporterDictionary.TryRemove(key, out _);
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }

        protected virtual void Execution_TestOutputEvent(MessageHandlerArgs<ITestOutput> args)
        {
            try
            {
                var testEvent = args.Message;
                string key = testEvent.Test.TestCase.UniqueID;

                ITestReporter testReporter = TestReporterDictionary[key];

                bool isInternalSharedMessage = false;

                try
                {
                    var communicationMessage = JsonSerializer.Deserialize<BaseCommunicationMessage>(testEvent.Output);

                    switch (communicationMessage.Action)
                    {
                        case CommunicationAction.AddLog:
                            var addLogCommunicationAction = JsonSerializer.Deserialize<AddLogCommunicationMessage>(testEvent.Output);
                            HandleAddLogCommunicationAction(testReporter, addLogCommunicationAction);
                            break;
                        case CommunicationAction.BeginLogScope:
                            var beginScopeCommunicationAction = JsonSerializer.Deserialize<BeginScopeCommunicationMessage>(testEvent.Output);
                            HandleBeginScopeCommunicationAction(testReporter, beginScopeCommunicationAction);
                            break;
                        case CommunicationAction.EndLogScope:
                            var endScopeCommunicationMessage = JsonSerializer.Deserialize<EndScopeCommunicationMessage>(testEvent.Output);
                            HandleEndScopeCommunicationMessage(endScopeCommunicationMessage);
                            break;
                    }

                    isInternalSharedMessage = true;
                }
                catch (Exception) { }

                if (!isInternalSharedMessage)
                {
                    testReporter.Log(new CreateLogItemRequest
                    {
                        Level = LogLevel.Info,
                        Time = DateTime.UtcNow,
                        Text = testEvent.Output
                    });
                }
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }

        // key: scope id, value: according test reporter
        private Dictionary<string, ITestReporter> _nestedScopes = new Dictionary<string, ITestReporter>();

        private void HandleAddLogCommunicationAction(ITestReporter testReporter, AddLogCommunicationMessage logMessage)
        {
            var logRequest = new CreateLogItemRequest
            {
                Text = logMessage.Text,
                Time = logMessage.Time,
                Level = logMessage.Level
            };

            if (logMessage.Attach != null)
            {
                logRequest.Attach = new LogItemAttach(logMessage.Attach.MimeType, logMessage.Attach.Data);
            }

            if (logMessage.ParentScopeId != null)
            {
                testReporter = _nestedScopes[logMessage.ParentScopeId];
            }

            testReporter.Log(logRequest);
        }

        private void HandleBeginScopeCommunicationAction(ITestReporter testReporter, BeginScopeCommunicationMessage logScopeMessage)
        {
            var startNestedStepRequest = new StartTestItemRequest
            {
                Name = logScopeMessage.Name,
                StartTime = logScopeMessage.BeginTime,
                Type = TestItemType.Step,
                HasStats = false
            };

            if (logScopeMessage.ParentScopeId != null)
            {
                testReporter = _nestedScopes[logScopeMessage.ParentScopeId];
            }

            var nestedStep = testReporter.StartChildTestReporter(startNestedStepRequest);
            _nestedScopes[logScopeMessage.Id] = nestedStep;
        }

        private Dictionary<Shared.Execution.Logging.LogScopeStatus, Status> _nestedStepStatusMap = new Dictionary<Shared.Execution.Logging.LogScopeStatus, Status> {
            { Shared.Execution.Logging.LogScopeStatus.InProgress, Status.InProgress },
            { Shared.Execution.Logging.LogScopeStatus.Passed, Status.Passed },
            { Shared.Execution.Logging.LogScopeStatus.Failed, Status.Failed },
            { Shared.Execution.Logging.LogScopeStatus.Skipped,Status.Skipped }
        };

        private void HandleEndScopeCommunicationMessage(EndScopeCommunicationMessage endScopeMessage)
        {
            var finishNestedStepRequest = new FinishTestItemRequest
            {
                EndTime = endScopeMessage.EndTime,
                Status = _nestedStepStatusMap[endScopeMessage.Status]
            };

            _nestedScopes[endScopeMessage.Id].Finish(finishNestedStepRequest);
            _nestedScopes.Remove(endScopeMessage.Id);
        }
    }
}