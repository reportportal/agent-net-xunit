using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Shared;
using ReportPortal.Shared.Extensibility;
using ReportPortal.Shared.Internal.Logging;
using ReportPortal.Shared.Logging;
using ReportPortal.XUnitReporter.LogHandler.Messages;
using System;
using System.Collections.Concurrent;
using System.IO;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter.LogHandler
{
    public class LogHandler : ILogHandler
    {
        private static readonly ITraceLogger TraceLogger;

        private static ConcurrentDictionary<ILogScope, ITestOutputHelper> _outputHelperMap = new ConcurrentDictionary<ILogScope, ITestOutputHelper>();

        static LogHandler()
        {
            var currentDirectory = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ReportPortalReporter).Assembly.Location).LocalPath));

            TraceLogger = TraceLogManager.Instance.WithBaseDir(currentDirectory).GetLogger<LogHandler>();

        }

        public static ITestOutputHelper XunitTestOutputHelper
        {
            get
            {
                return _outputHelperMap[Log.RootScope];
            }
            set
            {
                var rootScope = Log.RootScope;

                TraceLogger.Verbose($"Fixture is helping to assign ITestOutputHelper with current root scope {rootScope.GetHashCode()}...");

                _outputHelperMap[rootScope] = value;
            }
        }

        public int Order => 10;

        public void BeginScope(ILogScope logScope)
        {

            if (_outputHelperMap.TryGetValue(Log.RootScope, out ITestOutputHelper output))
            {
                var communicationMessage = new BeginScopeCommunicationMessage
                {
                    Id = logScope.Id,
                    ParentScopeId = logScope.Parent?.Id,
                    Name = logScope.Name,
                    BeginTime = logScope.BeginTime
                };

                output.WriteLine(Client.Converters.ModelSerializer.Serialize<BeginScopeCommunicationMessage>(communicationMessage));
            }
        }

        public void EndScope(ILogScope logScope)
        {
            var communicationMessage = new EndScopeCommunicationMessage
            {
                Id = logScope.Id,
                EndTime = logScope.EndTime.Value,
                Status = logScope.Status
            };

            _outputHelperMap[Log.RootScope].WriteLine(Client.Converters.ModelSerializer.Serialize<EndScopeCommunicationMessage>(communicationMessage));
        }

        public bool Handle(ILogScope logScope, CreateLogItemRequest logRequest)
        {
            var rootScope = Log.RootScope;

            TraceLogger.Verbose($"Handling log message for {rootScope.GetHashCode()} root scope...");

            if (_outputHelperMap.TryGetValue(rootScope, out ITestOutputHelper output))
            {
                var sharedLogMessage = new AddLogCommunicationMessage
                {
                    ParentScopeId = logScope?.Id,
                    Level = logRequest.Level,
                    Time = logRequest.Time,
                    Text = logRequest.Text
                };

                if (logRequest.Attach != null)
                {
                    sharedLogMessage.Attach = new Attach(logRequest.Attach.Name, logRequest.Attach.MimeType, logRequest.Attach.Data);
                }

                output.WriteLine(Client.Converters.ModelSerializer.Serialize<AddLogCommunicationMessage>(sharedLogMessage));
            }

            return true;
        }
    }
}
