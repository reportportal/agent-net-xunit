[![Build status](https://ci.appveyor.com/api/projects/status/2ltljpbft1ofxr32/branch/master?svg=true)](https://ci.appveyor.com/project/nvborisenko/agent-net-xunit/branch/master)

There are 2 ways to use ReportPortal with xUnit framework, it's depent on runner.

- [Visual Studio Runner](#visual-studio-runner)
- [xunit.console.exe](#xunitconsoleexe)

# Visual Studio Runner

This way is applicable if you use xunit with `xunit.runner.visualstudio` nuget package.

## Installation
Install `ReportPortal.XUnit` nuget package in project with xunit tests.

[![NuGet Badge](https://buildstats.info/nuget/reportportal.xunit)](https://www.nuget.org/packages/reportportal.xunit)

## Configuration
Add `ReportPortal.config.json` file to the test project.

```json
{
  "$schema": "https://raw.githubusercontent.com/reportportal/agent-net-xunit/master/src/ReportPortal.XUnitReporter/ReportPortal.config.schema",
  "enabled": true,
  "server": {
    "url": "https://rp.epam.com/api/v1/",
    "project": "default_project",
    "authentication": {
      "uuid": "aa19555c-c9ce-42eb-bb11-87757225d535"
    }
  },
  "launch": {
    "name": "XUnit Demo Launch",
    "description": "this is description",
    "debugMode": false,
    "attributes": [ "t1", "os:win10" ]
  }
}
```

## Run tests
Now if you execute tests via `dotnet test`, or `dotnet vstest`, or `vstest.console.exe`, you should see real-time report.

# xunit.console.exe

If you execute tests with `xunit.console.exe` runner.

## Installation
Download zip archive from the `Releases` tab and extract it into the folder with `xunit.console.exe`. After downloading zip file, make sure Windows didn't block it: right click on zip -> Properties -> Unblock.

2 files should be placed in the same folder:
- xunit.console.exe
- ReportPortal.XUnitReporter.dll

> Note: Supports only xUnit v2.4.1. Awaiting [issue](https://github.com/xunit/xunit/issues/1874) with ability to use custom reporters.

To verify whether reporter is available, execute `xunit.console.exe` without parameters. `-reportportal` should be listed in Reporters section.

## Configuration
Configure connection with Report Portal server in `ReportPortal.config.json` file. Sample is already included in zip archive.

## Run tests
Just execute your tests as you do it usually. Test results are automatically will be sent during execution.

# Environment variables
It's possible to override parameters via environment variables.
```cmd
set reportportal_launch_name="My new launch name"
# execute tests
```

`reportportal_` prefix is used for naming variables, and `_` is used as delimeter. For example to override `Server.Authentication.Uuid` parameter, we need specify `ReportPortal_Server_Authentication_Uuid` in environment variables. To override launch tags we need specify `ReportPortal_Launch_Attributes` with `tag1;tag2` value (`;` used as separator for list of values).

# Integrate logger framework
- [NLog](https://github.com/reportportal/logger-net-nlog)
- [log4net](https://github.com/reportportal/logger-net-log4net)
- [Serilog](https://github.com/reportportal/logger-net-serilog)
- [System.Diagnostics.TraceListener](https://github.com/reportportal/logger-net-tracelistener)

By default xunit doesn't have capturing test output mechanism. To make log frameworks to put messages to ReportPortal, you have to declare `ITestOutputHelper` object in fixture class constructor, and attach ReportPortal to it.

```csharp
class MyTests
{
  private ITestOutputHelper _output;

  void MyTests(ITestOutputHelper output)
  {
    _output = output.WithReportPortal();
  }

  [Fact]
  public void MyTest1()
  {
    _output.WriteLine("my message"); // this message goes to test output, will see it at the end of test
    ReportPortal.Shared.Log.Info("my message"); // this message goes immediately to Report Portal

    // or use log framework to produce messages
  }
}
```

And [how](https://github.com/reportportal/commons-net/blob/master/docs/Logging.md) you can improve your logging experience with attachments or nested steps.


# Useful extensions
- [SourceBack](https://github.com/nvborisenko/reportportal-extensions-sourceback) adds piece of test code where test was failed
- [Insider](https://github.com/nvborisenko/reportportal-extensions-insider) brings more reporting capabilities without coding like methods invocation as nested steps


# License
ReportPortal is licensed under [Apache 2.0](https://github.com/reportportal/agent-net-xunit/blob/master/LICENSE)

We use Google Analytics for sending anonymous usage information as library's name/version and the agent's name/version when starting launch. This information might help us to improve integration with ReportPortal. Used by the ReportPortal team only and not for sharing with 3rd parties. You are able to [turn off](https://github.com/reportportal/commons-net/blob/master/docs/Configuration.md#analytics) it if needed.
