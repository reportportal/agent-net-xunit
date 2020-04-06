using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Shared;
using ReportPortal.Shared.Extensibility;
using ReportPortal.Shared.Logging;
using ReportPortal.XUnitReporter.LogHandler.Messages;
using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter.LogHandler
{
    public class LogHandler : ILogHandler
    {
        private static ConcurrentDictionary<ILogScope, ITestOutputHelper> _outputHelper = new ConcurrentDictionary<ILogScope, ITestOutputHelper>();

        public static ITestOutputHelper XunitTestOutputHelper
        {
            get
            {
                return _outputHelper[Log.RootScope];
            }
            set
            {
                _outputHelper[Log.RootScope] = value;
            }
        }

        public int Order => 10;

        public void BeginScope(ILogScope logScope)
        {
            if (_outputHelper != null)
            {
                var communicationMessage = new BeginScopeCommunicationMessage
                {
                    Id = logScope.Id,
                    ParentScopeId = logScope.Parent?.Id,
                    Name = logScope.Name,
                    BeginTime = logScope.BeginTime
                };

                _outputHelper[Log.RootScope].WriteLine(Client.Converters.ModelSerializer.Serialize<BeginScopeCommunicationMessage>(communicationMessage));
            }
        }

        public void EndScope(ILogScope logScope)
        {
            if (_outputHelper != null)
            {
                var communicationMessage = new EndScopeCommunicationMessage
                {
                    Id = logScope.Id,
                    EndTime = logScope.EndTime.Value,
                    Status = logScope.Status
                };

                _outputHelper[Log.RootScope].WriteLine(Client.Converters.ModelSerializer.Serialize<EndScopeCommunicationMessage>(communicationMessage));
            }
        }

        public bool Handle(ILogScope logScope, CreateLogItemRequest logRequest)
        {
            if (_outputHelper != null)
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

                _outputHelper[Log.RootScope].WriteLine(Client.Converters.ModelSerializer.Serialize<AddLogCommunicationMessage>(sharedLogMessage));

                return true;
            }

            return false;
        }
    }
}
