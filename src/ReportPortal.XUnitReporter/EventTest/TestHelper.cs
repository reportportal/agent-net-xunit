using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        #region Helpers

        /// <summary>
        /// Compose error log in message for file.
        /// </summary>
        /// <param name="messageType">Message type.</param>
        /// <param name="failureInfo"></param>
        void LogError(string messageType, IFailureInformation failureInfo)
        {
            var message = $"[{messageType}] {failureInfo.ExceptionTypes[0]}: {ExceptionUtility.CombineMessages(failureInfo)}";
            var stack = ExceptionUtility.CombineStackTraces(failureInfo);
            LogImportant($"message text='{Escape(message)}' errorDetails='{Escape(stack)}' status='ERROR'");
        }

        /// <summary>
        /// Compose error log in message for report portal.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="failureInfo"></param>
        /// <returns>Composed message for write.</returns>
        string ErrorTestMessage(string messageType, IFailureInformation failureInfo)
        {
            var message = $"[{messageType}] {failureInfo.ExceptionTypes[0]}: {ExceptionUtility.CombineMessages(failureInfo)}";
            var stack = ExceptionUtility.CombineStackTraces(failureInfo);
            return $"message text='{Escape(message)}' errorDetails='{Escape(stack)}' status='ERROR'";
        }

        /// <summary>
        /// Compose finish log in message for file.
        /// </summary>
        /// <param name="testResult"></param>
        void LogFinish(ITestResultMessage testResult)
        {
            var formattedName = Escape(displayNameFormatter.DisplayName(testResult.Test));

            if (!string.IsNullOrWhiteSpace(testResult.Output))
                LogImportant($"Test StdOut name='{formattedName}' out='{Escape(testResult.Output)}'");

            LogImportant($"Test Finished name='{formattedName}' duration='{(int)(testResult.ExecutionTime * 1000M)}' flowId='{ToFlowId(testResult.TestCollection.DisplayName)}'");
        }

        /// <summary>
        /// Compose finish log in message for report portal.
        /// </summary>
        /// <param name="testResult"></param>
        /// <returns>Composed message for write.</returns>
        string FinishTestMessage(ITestResultMessage testResult)
        {
            var formattedName = Escape(displayNameFormatter.DisplayName(testResult.Test));
            string message = string.Empty;

            if (!string.IsNullOrWhiteSpace(testResult.Output))
                message += $"Test StdOut name='{formattedName}' out='{Escape(testResult.Output)}/n'";

            message += $"Test Finished name='{formattedName}' duration='{(int)(testResult.ExecutionTime * 1000M)}' flowId='{ToFlowId(testResult.TestCollection.DisplayName)}'";
            return message;
        }

        /// <summary>
        /// Write message in console and in .txt file.
        /// </summary>
        /// <param name="massage">Message for write.</param>
        void LogImportant(string massage)
        {
            logger.LogImportantMessage(massage);
            File.AppendAllText(path, massage + "\r\n");
        }

        static string Escape(string value)
        {
            if (value == null)
                return string.Empty;

            return value.Replace("|", "||")
                        .Replace("'", "|'")
                        .Replace("\r", "|r")
                        .Replace("\n", "|n")
                        .Replace("]", "|]")
                        .Replace("[", "|[")
                        .Replace("|r|n", "\r\n")
                        .Replace("\u0085", "|x")
                        .Replace("\u2028", "|l")
                        .Replace("\u2029", "|p");
        }

        string ToFlowId(string testCollectionName)
        {
            string result;

            flowMappingsLock.EnterReadLock();

            try
            {
                if (flowMappings.TryGetValue(testCollectionName, out result))
                    return result;
            }
            finally
            {
                flowMappingsLock.ExitReadLock();
            }

            flowMappingsLock.EnterWriteLock();

            try
            {
                if (!flowMappings.TryGetValue(testCollectionName, out result))
                {
                    result = flowIdMapper(testCollectionName);
                    flowMappings[testCollectionName] = result;
                }

                return result;
            }
            finally
            {
                flowMappingsLock.ExitWriteLock();
            }
        }
        #endregion
    }

}
