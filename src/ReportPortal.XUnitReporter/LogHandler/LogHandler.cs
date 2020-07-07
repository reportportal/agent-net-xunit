using ReportPortal.Shared;
using ReportPortal.Shared.Execution.Logging;
using ReportPortal.Shared.Extensibility;
using ReportPortal.Shared.Extensibility.Commands;
using ReportPortal.Shared.Internal.Logging;
using ReportPortal.XUnitReporter.LogHandler.Messages;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter.LogHandler
{
    public class LogHandler : ICommandsListener
    {
        private static readonly ITraceLogger TraceLogger;

        private static ConcurrentDictionary<ILogScope, ITestOutputHelper> _outputHelperMap = new ConcurrentDictionary<ILogScope, ITestOutputHelper>();

        public void Initialize(ICommandsSource commandsSource)
        {
            commandsSource.OnBeginLogScopeCommand += CommandsSource_OnBeginLogScopeCommand;
            commandsSource.OnEndLogScopeCommand += CommandsSource_OnEndLogScopeCommand;
            commandsSource.OnLogMessageCommand += CommandsSource_OnLogMessageCommand;
        }

        static LogHandler()
        {
            var currentDirectory = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ReportPortalReporter).Assembly.Location).LocalPath));

            TraceLogger = TraceLogManager.Instance.WithBaseDir(currentDirectory).GetLogger<LogHandler>();
        }

        public static ITestOutputHelper XunitTestOutputHelper
        {
            get
            {
                return _outputHelperMap[Context.Current.Log.Root];
            }
            set
            {
                var rootScope = Context.Current.Log.Root;

                TraceLogger.Verbose($"Fixture is helping to assign ITestOutputHelper with current root scope {rootScope.GetHashCode()}...");

                _outputHelperMap[rootScope] = value;
            }
        }

        private void CommandsSource_OnLogMessageCommand(Shared.Execution.ILogContext logContext, Shared.Extensibility.Commands.CommandArgs.LogMessageCommandArgs args)
        {
            var rootScope = Context.Current.Log.Root;

            TraceLogger.Verbose($"Handling log message for {rootScope.GetHashCode()} root scope...");

            var logScope = args.LogScope;

            if (_outputHelperMap.TryGetValue(rootScope, out ITestOutputHelper output))
            {
                var logRequest = args.LogMessage.ConvertToRequest();

                var sharedLogMessage = new AddLogCommunicationMessage
                {
                    ParentScopeId = logScope?.Id,
                    Level = logRequest.Level,
                    Time = logRequest.Time,
                    Text = logRequest.Text
                };

                if (logRequest.Attach != null)
                {
                    sharedLogMessage.Attach = new Attach(logRequest.Attach.MimeType, logRequest.Attach.Data);
                }

                NotifyAgent(output, Client.Converters.ModelSerializer.Serialize<AddLogCommunicationMessage>(sharedLogMessage));
            }
        }

        private void CommandsSource_OnEndLogScopeCommand(Shared.Execution.ILogContext logContext, Shared.Extensibility.Commands.CommandArgs.LogScopeCommandArgs args)
        {
            var logScope = args.LogScope;

            var communicationMessage = new EndScopeCommunicationMessage
            {
                Id = logScope.Id,
                EndTime = logScope.EndTime.Value,
                Status = logScope.Status
            };

            var outputHelper = _outputHelperMap[Context.Current.Log.Root];

            NotifyAgent(outputHelper, Client.Converters.ModelSerializer.Serialize<EndScopeCommunicationMessage>(communicationMessage));
        }

        private void CommandsSource_OnBeginLogScopeCommand(Shared.Execution.ILogContext logContext, Shared.Extensibility.Commands.CommandArgs.LogScopeCommandArgs args)
        {
            var logScope = args.LogScope;

            if (_outputHelperMap.TryGetValue(Context.Current.Log.Root, out ITestOutputHelper output))
            {
                var communicationMessage = new BeginScopeCommunicationMessage
                {
                    Id = logScope.Id,
                    ParentScopeId = logScope.Parent?.Id,
                    Name = logScope.Name,
                    BeginTime = logScope.BeginTime
                };

                NotifyAgent(output, Client.Converters.ModelSerializer.Serialize<BeginScopeCommunicationMessage>(communicationMessage));
            }
        }

        private void NotifyAgent(ITestOutputHelper outputHelper, string serializedMessage)
        {
            var type = outputHelper.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = testMember.GetValue(outputHelper);

            var messageBusMember = type.GetField("messageBus", BindingFlags.Instance | BindingFlags.NonPublic);
            var messageBusValue = messageBusMember.GetValue(outputHelper);

            var messageBusType = messageBusValue.GetType();
            var m = messageBusType.GetMethod("QueueMessage", BindingFlags.Instance | BindingFlags.Public);
            TestOutput mmm = new TestOutput((ITest)test, serializedMessage);
            m.Invoke(messageBusValue, new object[] { mmm });
        }
    }
}
