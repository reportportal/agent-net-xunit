using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Shared.Extensibility;
using System;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter.LogHandler
{
    public class LogHandler : ILogHandler
    {
        [ThreadStatic]
        private static ITestOutputHelper _outputHelper;

        public static ITestOutputHelper XunitTestOutputHelper
        {
            get
            {
                return _outputHelper;
            }
            set
            {
                _outputHelper = value;
            }
        }

        public int Order => 10;

        public bool Handle(CreateLogItemRequest logRequest)
        {
            if (_outputHelper != null)
            {
                var sharedLogMessage = new SharedLogMessage
                {
                    Level = logRequest.Level,
                    Time = logRequest.Time,
                    Text = logRequest.Text
                };

                if (logRequest.Attach != null)
                {
                    sharedLogMessage.Attach = new Attach(logRequest.Attach.Name, logRequest.Attach.MimeType, logRequest.Attach.Data);
                }

                _outputHelper.WriteLine(Client.Converters.ModelSerializer.Serialize<SharedLogMessage>(sharedLogMessage));

                return true;
            }

            return false;
        }
    }
}
