﻿using ReportPortal.Client.Abstractions.Models;
using ReportPortal.Client.Abstractions.Requests;
using ReportPortal.Shared.Reporter;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ReportPortal.XUnitReporter
{
    public partial class ReportPortalReporterMessageHandler
    {
        /// <summary>
        /// Starting test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleTestCollectionStarting(MessageHandlerArgs<ITestCollectionStarting> args)
        {
            try
            {
                var testCollection = args.Message;
                string key = testCollection.TestCollection.UniqueID.ToString();

                ITestReporter testReporter = _launchReporter.StartChildTestReporter(
                    new StartTestItemRequest()
                    {
                        Name = testCollection.TestCollection.DisplayName,
                        StartTime = DateTime.UtcNow,
                        Type = TestItemType.Suite
                    });

                TestReporterDictionary[key] = testReporter;
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }

        /// <summary>
        /// Finishing test suite in report portal.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void HandleTestCollectionFinished(MessageHandlerArgs<ITestCollectionFinished> args)
        {
            try
            {
                var testCollection = args.Message;
                string key = testCollection.TestCollection.UniqueID.ToString();

                TestReporterDictionary[key].Finish(new FinishTestItemRequest()
                {
                    EndTime = DateTime.UtcNow,
                    Status = testCollection.TestsFailed > 0 ? Status.Failed : Status.Passed
                });
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
            }
        }
    }
}
